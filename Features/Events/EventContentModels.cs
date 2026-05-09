using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace MyProject.Features.Events;

public static class EventContentAliases
{
    public const string EventsLandingPage = "eventsLandingPage";
    public const string EventPage = "eventPage";
    public const string PageTitle = "pageTitle";
    public const string IntroHeading = "introHeading";
    public const string IntroSummary = "introSummary";
    public const string EmptyStateHeading = "emptyStateHeading";
    public const string EmptyStateBody = "emptyStateBody";
    public const string EventTitle = "eventTitle";
    public const string EventDate = "eventDate";
    public const string VenueName = "venueName";
    public const string Summary = "summary";
    public const string HeroImage = "heroImage";
    public const string BodyContent = "bodyContent";
    public const string ArtistLineup = "artistLineup";
    public const string TicketCtaLabel = "ticketCtaLabel";
    public const string TicketCtaUrl = "ticketCtaUrl";
    public const string SecondarySupportingImage = "secondarySupportingImage";
}

public sealed record EventsLandingPageViewModel(
    string PageTitle,
    string IntroHeading,
    string? IntroSummary,
    string EmptyStateHeading,
    string EmptyStateBody,
    IReadOnlyList<EventCardViewModel> UpcomingEvents);

public sealed record EventCardViewModel(
    Guid ContentKey,
    string Title,
    DateTime EventDate,
    string VenueName,
    string Summary,
    string HeroImageUrl,
    string HeroImageAlt,
    string DetailUrl,
    string? ArtistLineup,
    string? TicketCtaLabel,
    string? TicketCtaUrl);

public sealed record EventDetailViewModel(
    Guid ContentKey,
    string Title,
    DateTime EventDate,
    string VenueName,
    string Summary,
    string HeroImageUrl,
    string HeroImageAlt,
    IHtmlContent BodyContent,
    string? ArtistLineup,
    string? TicketCtaLabel,
    string? TicketCtaUrl,
    string? SecondaryImageUrl,
    string SecondaryImageAlt,
    string BackLinkUrl,
    string BackLinkLabel);

public static class EventContentMapper
{
    public static EventsLandingPageViewModel MapLandingPage(IPublishedContent content)
    {
        string pageTitle = content.Value<string>(EventContentAliases.PageTitle) ?? content.Name;
        string introHeading = content.Value<string>(EventContentAliases.IntroHeading) ?? pageTitle;
        string? introSummary = content.Value<string>(EventContentAliases.IntroSummary);
        string emptyStateHeading = content.Value<string>(EventContentAliases.EmptyStateHeading) ?? "No upcoming events";
        string emptyStateBody = content.Value<string>(EventContentAliases.EmptyStateBody) ?? "Check back soon for the next show announcement.";

        IReadOnlyList<EventCardViewModel> upcomingEvents = content
            .Children()
            .Where(child => child.ContentType.Alias.InvariantEquals(EventContentAliases.EventPage))
            .Select(MapEventCard)
            .Where(mapped => mapped is not null)
            .Select(mapped => mapped!)
            .Where(mapped => mapped.EventDate.Date >= DateTime.Today)
            .OrderBy(mapped => mapped.EventDate)
            .ToList();

        return new EventsLandingPageViewModel(pageTitle, introHeading, introSummary, emptyStateHeading, emptyStateBody, upcomingEvents);
    }

    public static EventDetailViewModel? MapEventDetail(IPublishedContent content)
    {
        if (!content.ContentType.Alias.InvariantEquals(EventContentAliases.EventPage))
        {
            return null;
        }

        string title = content.Value<string>(EventContentAliases.EventTitle) ?? content.Name;
        DateTime? eventDate = content.Value<DateTime?>(EventContentAliases.EventDate);
        string? venueName = content.Value<string>(EventContentAliases.VenueName);
        string? summary = content.Value<string>(EventContentAliases.Summary);
        IHtmlContent? bodyContent = ResolveBodyContent(content);
        IPublishedContent? heroImage = content.Value<IPublishedContent>(EventContentAliases.HeroImage);

        if (eventDate is null || string.IsNullOrWhiteSpace(venueName) || string.IsNullOrWhiteSpace(summary) || bodyContent is null || heroImage is null)
        {
            return null;
        }

        string heroImageUrl = heroImage.Url();
        if (string.IsNullOrWhiteSpace(heroImageUrl))
        {
            return null;
        }

        IPublishedContent? secondaryImage = content.Value<IPublishedContent>(EventContentAliases.SecondarySupportingImage);
        string? secondaryImageUrl = secondaryImage?.Url();

        IPublishedContent? parent = content.Parent<IPublishedContent>();

        return new EventDetailViewModel(
            content.Key,
            title,
            eventDate.Value,
            venueName,
            summary,
            heroImageUrl,
            heroImage.Name,
            bodyContent,
            content.Value<string>(EventContentAliases.ArtistLineup),
            ResolveTicketLabel(content),
            content.Value<string>(EventContentAliases.TicketCtaUrl),
            string.IsNullOrWhiteSpace(secondaryImageUrl) ? null : secondaryImageUrl,
            secondaryImage?.Name ?? title,
            parent?.Url() ?? "/",
            parent?.Name ?? "Back to events");
    }

    private static IHtmlContent? ResolveBodyContent(IPublishedContent content)
    {
        IHtmlContent? htmlContent = content.Value<IHtmlContent>(EventContentAliases.BodyContent);
        if (htmlContent is not null)
        {
            return htmlContent;
        }

        string? fallbackMarkup = content.Value<string>(EventContentAliases.BodyContent);
        if (string.IsNullOrWhiteSpace(fallbackMarkup))
        {
            return null;
        }

        return new HtmlString(fallbackMarkup);
    }

    private static EventCardViewModel? MapEventCard(IPublishedContent content)
    {
        string title = content.Value<string>(EventContentAliases.EventTitle) ?? content.Name;
        DateTime? eventDate = content.Value<DateTime?>(EventContentAliases.EventDate);
        string? venueName = content.Value<string>(EventContentAliases.VenueName);
        string? summary = content.Value<string>(EventContentAliases.Summary);
        IPublishedContent? heroImage = content.Value<IPublishedContent>(EventContentAliases.HeroImage);

        if (eventDate is null || string.IsNullOrWhiteSpace(venueName) || string.IsNullOrWhiteSpace(summary) || heroImage is null)
        {
            return null;
        }

        string heroImageUrl = heroImage.Url();
        if (string.IsNullOrWhiteSpace(heroImageUrl))
        {
            return null;
        }

        return new EventCardViewModel(
            content.Key,
            title,
            eventDate.Value,
            venueName,
            summary,
            heroImageUrl,
            heroImage.Name,
            content.Url(),
            content.Value<string>(EventContentAliases.ArtistLineup),
            ResolveTicketLabel(content),
            content.Value<string>(EventContentAliases.TicketCtaUrl));
    }

    private static string? ResolveTicketLabel(IPublishedContent content)
    {
        string? ticketUrl = content.Value<string>(EventContentAliases.TicketCtaUrl);
        if (string.IsNullOrWhiteSpace(ticketUrl))
        {
            return null;
        }

        return content.Value<string>(EventContentAliases.TicketCtaLabel) ?? "Get Tickets";
    }
}