import { UMB_CONTENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/content';
import { css, html, nothing, repeat } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

export class MyProjectEventRegistrationsWorkspaceViewElement extends UmbLitElement {
  #workspaceContext;
  #refreshHandle;

  static properties = {
    _eventKey: { state: true },
    _loading: { state: true },
    _error: { state: true },
    _rows: { state: true },
    _lastUpdated: { state: true },
  };

  constructor() {
    super();

    this._eventKey = undefined;
    this._loading = false;
    this._error = '';
    this._rows = [];
    this._lastUpdated = '';

    this.consumeContext(UMB_CONTENT_WORKSPACE_CONTEXT, (context) => {
      this.#workspaceContext = context;

      this.observe(this.#workspaceContext?.unique, (unique) => {
        const eventKey = typeof unique === 'string' && unique.trim().length > 0 ? unique.trim() : undefined;

        if (eventKey === this._eventKey) {
          return;
        }

        this._eventKey = eventKey;
        this.#restartPolling();
        void this.#loadRegistrations();
      }, 'event-registrations-workspace-view');
    });
  }

  connectedCallback() {
    super.connectedCallback();
    this.#restartPolling();
  }

  disconnectedCallback() {
    super.disconnectedCallback();
    this.#stopPolling();
  }

  #restartPolling() {
    this.#stopPolling();

    if (!this._eventKey) {
      return;
    }

    this.#refreshHandle = window.setInterval(() => {
      void this.#loadRegistrations();
    }, 15000);
  }

  #stopPolling() {
    if (this.#refreshHandle !== undefined) {
      window.clearInterval(this.#refreshHandle);
      this.#refreshHandle = undefined;
    }
  }

  async #loadRegistrations() {
    const eventKey = this._eventKey;

    if (!eventKey) {
      this._rows = [];
      this._error = '';
      this._loading = false;
      this._lastUpdated = '';
      this.#stopPolling();
      return;
    }

    this._loading = true;
    this._error = '';

    try {
      const response = await fetch(`/umbraco/backoffice/registrations/api?eventKey=${encodeURIComponent(eventKey)}`, {
        headers: {
          Accept: 'application/json',
        },
        credentials: 'same-origin',
      });

      if (!response.ok) {
        throw new Error(
          response.status === 401
            ? 'Sign in to view registrations.'
            : `Unable to load registrations (${response.status}).`,
        );
      }

      const data = await response.json();
      this._rows = Array.isArray(data) ? data : [];
      this._lastUpdated = new Intl.DateTimeFormat([], {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      }).format(new Date());
    } catch (error) {
      this._rows = [];
      this._error = error instanceof Error ? error.message : 'Unable to load registrations.';
    } finally {
      this._loading = false;
    }
  }

  #formatUtc(value) {
    return new Intl.DateTimeFormat([], {
      dateStyle: 'medium',
      timeStyle: 'short',
      timeZone: 'UTC',
    }).format(new Date(value));
  }

  #onRefreshClick() {
    void this.#loadRegistrations();
  }

  render() {
    return html`
      <uui-box>
        <div class="shell">
          <div class="heading">
            <div>
              <h3>Live participant table</h3>
              <p class="muted" style="color: #fff !important;">
                ${this._eventKey
                  ? html` `
                  : 'Open a saved EventPage to load registrations.'}
              </p>
            </div>
            <div class="actions">
              <button type="button" @click=${this.#onRefreshClick}>Refresh</button>
            </div>
          </div>

          ${this._error ? html`<div class="state error">${this._error}</div>` : nothing}
          ${this._loading ? html`<div class="state loading">Loading registrations...</div>` : nothing}

          ${!this._loading && !this._error && !this._eventKey
            ? html`<div class="state empty">Save the EventPage to view registrations here.</div>`
            : nothing}

          ${!this._loading && !this._error && this._eventKey && this._rows.length === 0
            ? html`<div class="state empty"style="color: #fff !important;">No registrations have been submitted for this event yet.</div>`
            : nothing}

          ${this._rows.length > 0
            ? html`
                <div class="table-wrap">
                  <table>
                    <thead>
                      <tr >
                        <th style="color: #fff !important;">Created (UTC)</th>
                        <th style="color: #fff !important;">Name</th>
                        <th style="color: #fff !important;">Email</th>
                        <th style="color: #fff !important;">Phone</th>
                        <th style="color: #fff !important;">Consent</th>
                      </tr>
                    </thead>
                    <tbody>
                      ${repeat(
                        this._rows,
                        (row) => row.id,
                        (row) => html`
                          <tr>
                            <td>${this.#formatUtc(row.createdUtc)}</td>
                            <td>
                              <div class="primary">${row.name}</div>
                            </td>
                            <td>${row.email}</td>
                            <td>${row.phone ? row.phone : '-'}</td>
                            <td>${row.consentAccepted ? 'Yes' : 'No'}</td>
                          </tr>
                        `,
                      )}
                    </tbody>
                  </table>
                </div>
                ${this._lastUpdated ? html`<p class="muted updated">Last refreshed ${this._lastUpdated}</p>` : nothing}
              `
            : nothing}
        </div>
      </uui-box>
    `;
  }

  static styles = [
    css`
      :host {
        display: block;
        padding: var(--uui-size-space-5);
      }

      .shell {
        display: grid;
        gap: var(--uui-size-space-4);
      }

      .heading {
        display: flex;
        justify-content: space-between;
        gap: var(--uui-size-space-4);
        align-items: flex-start;
        flex-wrap: wrap;
      }

      .eyebrow {
        margin: 0 0 0.25rem;
        text-transform: uppercase;
        letter-spacing: 0.08em;
        font-size: 0.75rem;
        color: var(--uui-color-surface-emphasis);
      }

      h3 {
        margin: 0;
        font-size: 1.25rem;
      }

      .muted {
        margin: 0.35rem 0 0;
        color: var(--uui-color-surface-emphasis);
      }

      .actions {
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem;
        align-items: center;
      }

      button,
      a.secondary {
        border-radius: 999px;
        border: 1px solid var(--uui-color-border);
        background: var(--uui-color-surface);
        color: var(--uui-color-contrast);
        padding: 0.55rem 0.85rem;
        font: inherit;
        text-decoration: none;
        cursor: pointer;
      }

      .state {
        border-radius: 12px;
        padding: 0.85rem 1rem;
      }

      .state.empty,
      .state.loading {
        background: var(--uui-color-surface-alt);
        color: var(--uui-color-surface-emphasis);
      }

      .state.error {
        background: color-mix(in srgb, var(--uui-color-danger) 10%, transparent);
        color: var(--uui-color-danger-contrast, var(--uui-color-danger));
      }

      .table-wrap {
        overflow: auto;
        border: 1px solid var(--uui-color-border);
        border-radius: 12px;
      }

      table {
        width: 100%;
        border-collapse: collapse;
      }

      th,
      td {
        text-align: left;
        vertical-align: top;
        padding: 0.75rem 0.85rem;
        border-bottom: 1px solid var(--uui-color-border);
      }

      th {
        background: var(--uui-color-surface-alt);
        color: var(--uui-color-surface-emphasis);
        font-weight: 600;
        white-space: nowrap;
      }

      tr:last-child td {
        border-bottom: none;
      }

      .primary {
        font-weight: 600;
      }

      .secondary,
      .updated {
        color: var(--uui-color-surface-emphasis);
      }
    `,
  ];
}

customElements.define('myproject-event-registrations-workspace-view', MyProjectEventRegistrationsWorkspaceViewElement);

export { MyProjectEventRegistrationsWorkspaceViewElement as element };