using Microsoft.Data.Sqlite;
using System.Globalization;

namespace MyProject.Features.Events;

public interface IEventRegistrationService
{
    Task EnsureStoreAsync(CancellationToken cancellationToken = default);
    Task<EventRegistrationResult> RegisterAsync(EventRegistrationCreateRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventRegistrationRow>> ListAsync(Guid? eventKey = null, CancellationToken cancellationToken = default);
}

public sealed class EventRegistrationService : IEventRegistrationService
{
    private readonly string _connectionString;

    public EventRegistrationService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        string rawConnectionString = configuration.GetConnectionString("umbracoDbDSN")
            ?? throw new InvalidOperationException("Missing connection string 'umbracoDbDSN'.");

        string dataDirectory = Path.Combine(environment.ContentRootPath, "umbraco", "Data");
        _connectionString = rawConnectionString.Replace("|DataDirectory|", dataDirectory, StringComparison.OrdinalIgnoreCase);
    }

    public async Task EnsureStoreAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string createTableSql = """
            CREATE TABLE IF NOT EXISTS EventRegistrations (
                Id TEXT NOT NULL PRIMARY KEY,
                EventKey TEXT NOT NULL,
                EventName TEXT NOT NULL,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                EmailNormalized TEXT NOT NULL,
                Phone TEXT NULL,
                ConsentAccepted INTEGER NOT NULL,
                CreatedUtc TEXT NOT NULL
            );
            """;

        const string createIndexSql = """
            CREATE UNIQUE INDEX IF NOT EXISTS IX_EventRegistrations_EventKey_EmailNormalized
            ON EventRegistrations(EventKey, EmailNormalized);
            """;

        await using SqliteCommand createTable = connection.CreateCommand();
        createTable.CommandText = createTableSql;
        await createTable.ExecuteNonQueryAsync(cancellationToken);

        await using SqliteCommand createIndex = connection.CreateCommand();
        createIndex.CommandText = createIndexSql;
        await createIndex.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<EventRegistrationResult> RegisterAsync(EventRegistrationCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EventKey == Guid.Empty)
        {
            return new EventRegistrationResult(EventRegistrationStatus.Invalid, "Event reference is missing.");
        }

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return new EventRegistrationResult(EventRegistrationStatus.Invalid, "Name and email are required.");
        }

        if (!request.ConsentAccepted)
        {
            return new EventRegistrationResult(EventRegistrationStatus.Invalid, "Consent is required.");
        }

        string normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (!normalizedEmail.Contains('@', StringComparison.Ordinal))
        {
            return new EventRegistrationResult(EventRegistrationStatus.Invalid, "A valid email address is required.");
        }

        Guid registrationId = Guid.NewGuid();
        DateTime createdUtc = DateTime.UtcNow;

        try
        {
            await using SqliteConnection connection = new(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO EventRegistrations
                (Id, EventKey, EventName, Name, Email, EmailNormalized, Phone, ConsentAccepted, CreatedUtc)
                VALUES
                (@id, @eventKey, @eventName, @name, @email, @emailNormalized, @phone, @consentAccepted, @createdUtc);
                """;

            command.Parameters.AddWithValue("@id", registrationId.ToString("D", CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("@eventKey", request.EventKey.ToString("D", CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("@eventName", request.EventName.Trim());
            command.Parameters.AddWithValue("@name", request.Name.Trim());
            command.Parameters.AddWithValue("@email", request.Email.Trim());
            command.Parameters.AddWithValue("@emailNormalized", normalizedEmail);
            command.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(request.Phone) ? DBNull.Value : request.Phone.Trim());
            command.Parameters.AddWithValue("@consentAccepted", request.ConsentAccepted ? 1 : 0);
            command.Parameters.AddWithValue("@createdUtc", createdUtc.ToString("O", CultureInfo.InvariantCulture));

            await command.ExecuteNonQueryAsync(cancellationToken);

            EventRegistrationRow row = new(
                registrationId,
                request.EventKey,
                request.EventName.Trim(),
                request.Name.Trim(),
                request.Email.Trim(),
                string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                request.ConsentAccepted,
                createdUtc);

            return new EventRegistrationResult(EventRegistrationStatus.Success, "Registration completed.", row);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            return new EventRegistrationResult(EventRegistrationStatus.Duplicate, "You are already registered for this event with this email.");
        }
        catch
        {
            return new EventRegistrationResult(EventRegistrationStatus.Error, "Registration could not be completed. Please try again.");
        }
    }

    public async Task<IReadOnlyList<EventRegistrationRow>> ListAsync(Guid? eventKey = null, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = new(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = eventKey is null
            ? "SELECT Id, EventKey, EventName, Name, Email, Phone, ConsentAccepted, CreatedUtc FROM EventRegistrations ORDER BY CreatedUtc DESC;"
            : "SELECT Id, EventKey, EventName, Name, Email, Phone, ConsentAccepted, CreatedUtc FROM EventRegistrations WHERE EventKey = @eventKey ORDER BY CreatedUtc DESC;";

        if (eventKey is Guid filterKey)
        {
            command.Parameters.AddWithValue("@eventKey", filterKey.ToString("D", CultureInfo.InvariantCulture));
        }

        List<EventRegistrationRow> rows = new();
        await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            Guid id = Guid.Parse(reader.GetString(0));
            Guid rowEventKey = Guid.Parse(reader.GetString(1));
            string eventName = reader.GetString(2);
            string name = reader.GetString(3);
            string email = reader.GetString(4);
            string? phone = reader.IsDBNull(5) ? null : reader.GetString(5);
            bool consentAccepted = reader.GetInt64(6) == 1;
            DateTime createdUtc = DateTime.Parse(reader.GetString(7), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            rows.Add(new EventRegistrationRow(id, rowEventKey, eventName, name, email, phone, consentAccepted, createdUtc));
        }

        return rows;
    }
}
