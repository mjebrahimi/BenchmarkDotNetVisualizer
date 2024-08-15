using System.ComponentModel;
using System.Dynamic;
using System.Net;
using System.Text;

namespace BenchmarkDotNetVisualizer.Utilities;

/// <summary>
/// Html Helper for rendering html
/// </summary>
public static partial class HtmlHelper
{
    /// <summary>
    /// Converts the enumerable to HTML table document and saves as specified <paramref name="path"/> asynchronously.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="path">The path.</param>
    /// <param name="title">The title.</param>
    /// <param name="theme">The theme option</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task SaveAsHtmlTableDocumentAsync(this IEnumerable<ExpandoObject?> source, string path, string title, Theme theme = Theme.Dark,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple,
        CancellationToken cancellationToken = default)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = ToHtmlTableDocument(source, title, theme, dividerMode, htmlWrapMode);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, text, cancellationToken);
    }

    /// <summary>
    /// Converts the enumerable to HTML table document and saves as specified <paramref name="path"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="path">The path.</param>
    /// <param name="title">The title.</param>
    /// <param name="theme">The theme option</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    public static void SaveAsHtmlTableDocument(this IEnumerable<ExpandoObject?> source, string path, string title, Theme theme = Theme.Dark,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));
        ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));

        var text = ToHtmlTableDocument(source, title, theme, dividerMode, htmlWrapMode);
        DirectoryHelper.EnsureDirectoryExists(path);
        File.WriteAllText(path, text);
    }

    /// <summary>
    /// Converts specified enumerable to HTML table document.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="title">The title.</param>
    /// <param name="theme">The theme option</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <returns></returns>
    public static string ToHtmlTableDocument(this IEnumerable<ExpandoObject?> source, string title, Theme theme = Theme.Dark,
        RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        var table = ToHtmlTable(source, dividerMode);
        return WrapInHtmlDocument(table, title, theme, htmlWrapMode);
    }

    /// <summary>
    /// Wraps the body in HTML document and save as specified <paramref name="path"/> asynchronously.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="body">The body.</param>
    /// <param name="title">The title.</param>
    /// <param name="theme">The theme option</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task WrapInHtmlDocumentAndSaveAsAsync(string path, string body, string title, Theme theme = Theme.Dark,
        HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple, CancellationToken cancellationToken = default)
    {
        var html = WrapInHtmlDocument(body, title, theme, htmlWrapMode);
        DirectoryHelper.EnsureDirectoryExists(path);
        return File.WriteAllTextAsync(path, html, cancellationToken);
    }

    /// <summary>
    /// Wraps the body in HTML document and save as specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="body">The body.</param>
    /// <param name="title">The title.</param>
    /// <param name="theme">The theme option</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    public static void WrapInHtmlDocumentAndSaveAs(string path, string body, string title, Theme theme = Theme.Dark,
        HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        var html = WrapInHtmlDocument(body, title, theme, htmlWrapMode);
        DirectoryHelper.EnsureDirectoryExists(path);
        File.WriteAllText(path, html);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the created-by tag is disabled. (Defaults to <see langword="false"/>)
    /// </summary>
    /// <value>
    ///   <c>true</c> if you want to disable created-by tag; otherwise, <c>false</c>.
    /// </value>
    public static bool DisableCreatedBy { get; set; }
    private static string GetCreatedBy()
    {
        return DisableCreatedBy ? string.Empty :
            """<div class="created-by">Created by <a href="https://github.com/mjebrahimi/BenchmarkDotNetVisualizer"><strong>BenchmarkDotNetVisualizer</strong></a></div>""";
    }

    /// <summary>
    /// Wraps the body in HTML document and gets the output html.
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="title">The title.</param>
    /// <param name="theme">The theme</param>
    /// <param name="htmlWrapMode">The HTML wrap mode.</param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException">nameof(htmlWrapMode), Convert.ToInt32(htmlWrapMode), typeof(HtmlDocumentWrapMode)</exception>
    public static string WrapInHtmlDocument(string body, string title, Theme theme = Theme.Dark, HtmlDocumentWrapMode htmlWrapMode = HtmlDocumentWrapMode.Simple)
    {
        #region DataTables
        const string dataTablesCSS =
            """
            <link rel="stylesheet" href="https://cdn.datatables.net/1.13.7/css/jquery.dataTables.min.css" />
            <link rel="stylesheet" href="https://cdn.datatables.net/colreorder/1.5.4/css/colReorder.dataTables.min.css">
            <link rel="stylesheet" href="https://cdn.datatables.net/buttons/2.4.2/css/buttons.dataTables.min.css">
            """;

        const string dataTablesScript =
            """
            <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
            <script src="https://cdn.datatables.net/1.13.7/js/jquery.dataTables.min.js"></script>
            <script src="https://cdn.datatables.net/colreorder/1.5.4/js/dataTables.colReorder.min.js"></script>
            <script src="https://cdn.datatables.net/buttons/2.4.2/js/dataTables.buttons.min.js"></script>
            <script src="https://cdn.datatables.net/buttons/2.4.2/js/buttons.colVis.min.js"></script>
            <script>
              $(document).ready(function () {
                DataTable.ext.type.order["custom-orderer-pre"] = function (d) {
                  d = d.replace(/(<([^>]+)>)/gi, ""); //remove html tags
                  if (!d.match(/^\d/))
                    //if not started with number
                    return d;
                  var regex = /[\d,]+(?:\.\d+)?/;
                  var number = d.match(regex)[0].replace(/,/g, ""); //extract number
                  return parseFloat(number); //Number
                };
                $("table").DataTable({
                  info: false,
                  paging: false,
                  colReorder: true,
                  order: [], //remove default order
                  columnDefs: [
                    {
                      type: "custom-orderer",
                      targets: "_all",
                    },
                  ],
                  dom: "Bfrtip",
                  buttons: ["colvis"],
                });
              });
            </script>
            """;
        #endregion

        var dataTables = htmlWrapMode switch
        {
            HtmlDocumentWrapMode.Simple => (CSS: "", JS: ""),
            HtmlDocumentWrapMode.RichDataTables => (CSS: dataTablesCSS, JS: dataTablesScript),
            _ => throw new InvalidEnumArgumentException(nameof(htmlWrapMode), Convert.ToInt32(htmlWrapMode), typeof(HtmlDocumentWrapMode))
        };

        return
        $$$"""
        <!DOCTYPE html>
        <html lang='en' data-theme="{{{theme.ToString().ToLower()}}}">
        <head>
        <meta charset='utf-8' />
        <meta name="color-scheme" content="dark light">
        <title>{{{title}}}</title>
        {{{dataTables.CSS}}}
        <style type="text/css">
          :where(html){--ease-1:cubic-bezier(.25,0,.5,1);--ease-2:cubic-bezier(.25,0,.4,1);--ease-3:cubic-bezier(.25,0,.3,1);--ease-4:cubic-bezier(.25,0,.2,1);--ease-5:cubic-bezier(.25,0,.1,1);--ease-in-1:cubic-bezier(.25,0,1,1);--ease-in-2:cubic-bezier(.50,0,1,1);--ease-in-3:cubic-bezier(.70,0,1,1);--ease-in-4:cubic-bezier(.90,0,1,1);--ease-in-5:cubic-bezier(1,0,1,1);--ease-out-1:cubic-bezier(0,0,.75,1);--ease-out-2:cubic-bezier(0,0,.50,1);--ease-out-3:cubic-bezier(0,0,.3,1);--ease-out-4:cubic-bezier(0,0,.1,1);--ease-out-5:cubic-bezier(0,0,0,1);--ease-in-out-1:cubic-bezier(.1,0,.9,1);--ease-in-out-2:cubic-bezier(.3,0,.7,1);--ease-in-out-3:cubic-bezier(.5,0,.5,1);--ease-in-out-4:cubic-bezier(.7,0,.3,1);--ease-in-out-5:cubic-bezier(.9,0,.1,1);--ease-elastic-out-1:cubic-bezier(.5,.75,.75,1.25);--ease-elastic-out-2:cubic-bezier(.5,1,.75,1.25);--ease-elastic-out-3:cubic-bezier(.5,1.25,.75,1.25);--ease-elastic-out-4:cubic-bezier(.5,1.5,.75,1.25);--ease-elastic-out-5:cubic-bezier(.5,1.75,.75,1.25);--ease-elastic-in-1:cubic-bezier(.5,-0.25,.75,1);--ease-elastic-in-2:cubic-bezier(.5,-0.50,.75,1);--ease-elastic-in-3:cubic-bezier(.5,-0.75,.75,1);--ease-elastic-in-4:cubic-bezier(.5,-1.00,.75,1);--ease-elastic-in-5:cubic-bezier(.5,-1.25,.75,1);--ease-elastic-in-out-1:cubic-bezier(.5,-.1,.1,1.5);--ease-elastic-in-out-2:cubic-bezier(.5,-.3,.1,1.5);--ease-elastic-in-out-3:cubic-bezier(.5,-.5,.1,1.5);--ease-elastic-in-out-4:cubic-bezier(.5,-.7,.1,1.5);--ease-elastic-in-out-5:cubic-bezier(.5,-.9,.1,1.5);--ease-step-1:steps(2);--ease-step-2:steps(3);--ease-step-3:steps(4);--ease-step-4:steps(7);--ease-step-5:steps(10);--ease-elastic-1:var(--ease-elastic-out-1);--ease-elastic-2:var(--ease-elastic-out-2);--ease-elastic-3:var(--ease-elastic-out-3);--ease-elastic-4:var(--ease-elastic-out-4);--ease-elastic-5:var(--ease-elastic-out-5);--ease-squish-1:var(--ease-elastic-in-out-1);--ease-squish-2:var(--ease-elastic-in-out-2);--ease-squish-3:var(--ease-elastic-in-out-3);--ease-squish-4:var(--ease-elastic-in-out-4);--ease-squish-5:var(--ease-elastic-in-out-5);--ease-spring-1:linear(0,0.006,0.025 2.8%,0.101 6.1%,0.539 18.9%,0.721 25.3%,0.849 31.5%,0.937 38.1%,0.968 41.8%,0.991 45.7%,1.006 50.1%,1.015 55%,1.017 63.9%,1.001);--ease-spring-2:linear(0,0.007,0.029 2.2%,0.118 4.7%,0.625 14.4%,0.826 19%,0.902,0.962,1.008 26.1%,1.041 28.7%,1.064 32.1%,1.07 36%,1.061 40.5%,1.015 53.4%,0.999 61.6%,0.995 71.2%,1);--ease-spring-3:linear(0,0.009,0.035 2.1%,0.141 4.4%,0.723 12.9%,0.938 16.7%,1.017,1.077,1.121,1.149 24.3%,1.159,1.163,1.161,1.154 29.9%,1.129 32.8%,1.051 39.6%,1.017 43.1%,0.991,0.977 51%,0.974 53.8%,0.975 57.1%,0.997 69.8%,1.003 76.9%,1);--ease-spring-4:linear(0,0.009,0.037 1.7%,0.153 3.6%,0.776 10.3%,1.001,1.142 16%,1.185,1.209 19%,1.215 19.9% 20.8%,1.199,1.165 25%,1.056 30.3%,1.008 33%,0.973,0.955 39.2%,0.953 41.1%,0.957 43.3%,0.998 53.3%,1.009 59.1% 63.7%,0.998 78.9%,1);--ease-spring-5:linear(0,0.01,0.04 1.6%,0.161 3.3%,0.816 9.4%,1.046,1.189 14.4%,1.231,1.254 17%,1.259,1.257 18.6%,1.236,1.194 22.3%,1.057 27%,0.999 29.4%,0.955 32.1%,0.942,0.935 34.9%,0.933,0.939 38.4%,1 47.3%,1.011,1.017 52.6%,1.016 56.4%,1 65.2%,0.996 70.2%,1.001 87.2%,1);--ease-bounce-1:linear(0,0.004,0.016,0.035,0.063,0.098,0.141,0.191,0.25,0.316,0.391 36.8%,0.563,0.766,1 58.8%,0.946,0.908 69.1%,0.895,0.885,0.879,0.878,0.879,0.885,0.895,0.908 89.7%,0.946,1);--ease-bounce-2:linear(0,0.004,0.016,0.035,0.063,0.098,0.141 15.1%,0.25,0.391,0.562,0.765,1,0.892 45.2%,0.849,0.815,0.788,0.769,0.757,0.753,0.757,0.769,0.788,0.815,0.85,0.892 75.2%,1 80.2%,0.973,0.954,0.943,0.939,0.943,0.954,0.973,1);--ease-bounce-3:linear(0,0.004,0.016,0.035,0.062,0.098,0.141 11.4%,0.25,0.39,0.562,0.764,1 30.3%,0.847 34.8%,0.787,0.737,0.699,0.672,0.655,0.65,0.656,0.672,0.699,0.738,0.787,0.847 61.7%,1 66.2%,0.946,0.908,0.885 74.2%,0.879,0.878,0.879,0.885 79.5%,0.908,0.946,1 87.4%,0.981,0.968,0.96,0.957,0.96,0.968,0.981,1);--ease-bounce-4:linear(0,0.004,0.016 3%,0.062,0.141,0.25,0.391,0.562 18.2%,1 24.3%,0.81,0.676 32.3%,0.629,0.595,0.575,0.568,0.575,0.595,0.629,0.676 48.2%,0.811,1 56.2%,0.918,0.86,0.825,0.814,0.825,0.86,0.918,1 77.2%,0.94 80.6%,0.925,0.92,0.925,0.94 87.5%,1 90.9%,0.974,0.965,0.974,1);--ease-bounce-5:linear(0,0.004,0.016 2.5%,0.063,0.141,0.25 10.1%,0.562,1 20.2%,0.783,0.627,0.534 30.9%,0.511,0.503,0.511,0.534 38%,0.627,0.782,1 48.7%,0.892,0.815,0.769 56.3%,0.757,0.753,0.757,0.769 61.3%,0.815,0.892,1 68.8%,0.908 72.4%,0.885,0.878,0.885,0.908 79.4%,1 83%,0.954 85.5%,0.943,0.939,0.943,0.954 90.5%,1 93%,0.977,0.97,0.977,1)}
          .sun-and-moon>:is(.moon,.sun,.sun-beams){transform-origin:center center}.sun-and-moon>:is(.moon,.sun){fill:var(--icon-fill)}.theme-toggle:is(:hover,:focus-visible)>.sun-and-moon>:is(.moon,.sun){fill:var(--icon-fill-hover)}.sun-and-moon>.sun-beams{stroke:var(--icon-fill);stroke-width:2px}.theme-toggle:is(:hover,:focus-visible) .sun-and-moon>.sun-beams{stroke:var(--icon-fill-hover)}[data-theme=dark] .sun-and-moon>.sun{transform:scale(1.75)}[data-theme=dark] .sun-and-moon>.sun-beams{opacity:0}[data-theme=dark] .sun-and-moon>.moon>circle{transform:translate(-7px)}@supports (cx: 1){[data-theme=dark] .sun-and-moon>.moon>circle{transform:translate(0);cx:17}}@media (prefers-reduced-motion: no-preference){.sun-and-moon>.sun{transition:transform .5s var(--ease-elastic-3)}.sun-and-moon>.sun-beams{transition:transform .5s var(--ease-elastic-4),opacity .5s var(--ease-3)}.sun-and-moon .moon>circle{transition:transform .25s var(--ease-out-5)}@supports (cx: 1){.sun-and-moon .moon>circle{transition:cx .25s var(--ease-out-5)}}[data-theme=dark] .sun-and-moon>.sun{transform:scale(1.75);transition-timing-function:var(--ease-3);transition-duration:.25s}[data-theme=dark] .sun-and-moon>.sun-beams{transform:rotate(-25deg);transition-duration:.15s}[data-theme=dark] .sun-and-moon>.moon>circle{transition-delay:.25s;transition-duration:.5s}}.theme-toggle{position:absolute;right:0;margin:0.5rem 1rem;--size: 2rem;--icon-fill: hsl(210 10% 30%);--icon-fill-hover: hsl(210 10% 15%);background:none;border:none;padding:0;inline-size:var(--size);block-size:var(--size);aspect-ratio:1;border-radius:50%;cursor:pointer;touch-action:manipulation;-webkit-tap-highlight-color:transparent;outline-offset:5px}.theme-toggle>svg{inline-size:100%;block-size:100%;stroke-linecap:round}[data-theme=dark] .theme-toggle{--icon-fill: hsl(210 10% 70%);--icon-fill-hover: hsl(210 15% 90%) }@media (hover: none){.theme-toggle{--size: 48px}}
          html[data-theme="light"] {
            color-scheme: light;
            body { font-family: system-ui; padding: 0 30px 30px 30px; width: min-content; display: inline-block; background-color: #FFFFFF; color: #1F1F1F; }
            h1, h2 { border-bottom: solid 1px #CCCCCC;}
            h1, h2, h3 { padding-bottom: 0.3em; }
            table { border-collapse: collapse !important; margin-top: 3px !important; width: 100%; display: inline-table; margin-bottom: 20px !important; }
            td, th { padding: 6px 13px; border: 1px solid #CCCCCC; text-align: right; white-space: nowrap; color: #000000; }
            tr { background-color: #FFFFFF !important; border-top: 1px solid #CCCCCC; }
            tr:nth-child(even):not(.divider) { background: #F8F8F8 !important; }
            tr.divider { border: 0; font-size: 10px; }
            tr.divider td { padding: 0; border: 0; }
            pre { background: #EFEFEF; padding: 0 1em; color: #1F1F1F; }
            thead th { background-color: #EFEFEF; color: #1F1F1F; }
            tbody tr:not(.divider):hover { background-color: #EFEFEF !important; border: 2px solid #ADADAD; }
            .created-by { text-align: center; margin-bottom: -20px !important; font-weight: bold; }
            .dataTables_wrapper .dataTables_filter input {border: 1px solid #AAAAAA;}
            input[type="search"] { font-size: 1em !important; padding: 9px 12px !important; border: 1px solid #AAAAAA; outline: none;}
            input[type="search"]:hover,input[type="search"]:focus { border-color: #666; }
            .dataTable thead th:hover { background-color: #E0E0E0; }
            div.dt-buttons > .dt-button {border: 1px solid #AAAAAA;}
            div.dt-button-collection{background-color: #FFFFFF;}
          }
          html[data-theme="dark"] {
            color-scheme: dark;
            body { font-family: system-ui; padding: 0 30px 30px 30px; width: min-content; display: inline-block; background-color: #121212; color: #E0E0E0; }
            h1, h2 { border-bottom: solid 1px #444444; }
            h1, h2, h3 { padding-bottom: 0.3em; }
            table { border-collapse: collapse !important; margin-top: 3px !important; width: 100%; display: inline-table; margin-bottom: 20px !important; }
            td, th { padding: 6px 13px; border: 1px solid #444444; text-align: right; white-space: nowrap; color: black; }
            tr { background-color: #1E1E1E !important; border-top: 1px solid #444444; }
            tr:nth-child(even):not(.divider) { background: #262626 !important }
            tr.divider { border: 0; font-size: 10px; }
            tr.divider td { padding: 0; border: 0; }
            pre { background: #2E2E2E; padding: 0 1em; color: #E0E0E0; }
            thead th { background-color: #2E2E2E; color: #E0E0E0; }
            tbody tr:not(.divider):hover { background-color: #2E2E2E !important; border: 2px solid #777777; }
            .created-by { text-align: center; margin-bottom: -20px !important; font-weight: bold; }
            td:not([hasBackground]) { filter: invert(1); }
            input[type="search"] { font-size: 1em !important; padding: 9px 12px !important; border: 1px solid #444444; outline: none; }
            input[type="search"]:hover,input[type="search"]:focus { border-color: #666666; }
            .dataTable thead th:hover { background-color: #222222; }
            div.dt-buttons > .dt-button {border: 1px solid #444444;}
            div.dt-button-collection{background-color: #2B2B2B;}
          }
        </style>
        </head>
        <body>
        <button class="theme-toggle" id="theme-toggle" title="Toggles light & dark" aria-label="auto" aria-live="polite">
          <svg class="sun-and-moon" aria-hidden="true" width="24" height="24" viewBox="0 0 24 24">
          <mask class="moon" id="moon-mask">
            <rect x="0" y="0" width="100%" height="100%" fill="white" />
            <circle cx="24" cy="10" r="6" fill="black" />
          </mask>
          <circle class="sun" cx="12" cy="12" r="6" mask="url(#moon-mask)" fill="currentColor" />
          <g class="sun-beams" stroke="currentColor">
            <line x1="12" y1="1" x2="12" y2="3" />
            <line x1="12" y1="21" x2="12" y2="23" />
            <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
            <line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
            <line x1="1" y1="12" x2="3" y2="12" />
            <line x1="21" y1="12" x2="23" y2="12" />
            <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
            <line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
            </g>
          </svg>
        </button>
        {{{body}}}
        {{{GetCreatedBy()}}}
        </body>
        <script>
          const storageKey = "theme-preference";
          const getColorPreference = () => {
            if (localStorage.getItem(storageKey))
              return localStorage.getItem(storageKey);
            else
              return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
          };
          const theme = { value: getColorPreference() };
          const applyPreference = () => {
            document.firstElementChild.setAttribute("data-theme", theme.value);
            document.querySelector("#theme-toggle")?.setAttribute("aria-label", theme.value);
          };
          const setPreference = () => {
            localStorage.setItem(storageKey, theme.value);
            applyPreference();
          };
          const onClick = () => {
            theme.value = theme.value === "light" ? "dark" : "light";
            setPreference();
          };
          if (localStorage.getItem(storageKey) == null) {
            //set default theme if no preference is set
            theme.value = "{{{theme.ToString().ToLower()}}}";
            setPreference();
          }
          applyPreference();
          window.onload = () => {
            applyPreference();
            document.querySelector("#theme-toggle").addEventListener("click", onClick);
          };
          window
            .matchMedia("(prefers-color-scheme: dark)")
            .addEventListener("change", ({ matches: isDark }) => {
              theme.value = isDark ? "dark" : "light";
              setPreference();
            });
        </script>
        {{{dataTables.JS}}}
        </html>
        """;
    }

    /// <summary>
    /// Converts specified enumerable to HTML table (without whole document).
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dividerMode">The divider mode.</param>
    /// <returns></returns>
    public static string ToHtmlTable(this IEnumerable<ExpandoObject?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        return ToHtmlTableCore(source, dividerMode);
    }

    private static string ToHtmlTableCore(IEnumerable<ExpandoObject?> source, RenderTableDividerMode dividerMode = RenderTableDividerMode.EmptyDividerRow)
    {
        Guard.ThrowIfNullOrEmpty(source, nameof(source));

        var stringBuilder = new StringBuilder();
        switch (dividerMode)
        {
            case RenderTableDividerMode.Ignore:
            case RenderTableDividerMode.EmptyDividerRow:
            case var _ when source.Any(p => p is null) is false:
                Render(source);
                break;
            case RenderTableDividerMode.SeparateTables:
                foreach (var table in source.SplitByNull())
                    Render(table);
                break;
            default:
                throw new NotImplementedException();
        }
        return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());

        void Render(IEnumerable<ExpandoObject?> table)
        {
            var props = source.First()!.GetColumnsByOrder();

#pragma warning disable RCS1197 // Optimize StringBuilder.Append/AppendLine call
            stringBuilder.AppendLine("<table>");

            //Header
            stringBuilder.AppendLine("<thead>");
            stringBuilder.AppendLine("<tr>");
            foreach (var prop in props)
            {
                stringBuilder.Append($"<th>{prop}</th>");
            }
            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("</thead>");

            //Body
            stringBuilder.AppendLine("<tbody>");
            foreach (var item in table)
            {
                if (item is null)
                {
                    switch (dividerMode)
                    {
                        case RenderTableDividerMode.EmptyDividerRow:
                            stringBuilder.AppendLine(@"<tr class=""divider"">");
                            stringBuilder.AppendLine($@"<td colspan=""{props.Length}"">&nbsp;</td>");
                            stringBuilder.AppendLine("</tr>");

                            stringBuilder.AppendLine(@"<tr class=""divider"">");
                            stringBuilder.AppendLine($@"<td colspan=""{props.Length}""></td>");
                            stringBuilder.AppendLine("</tr>");
                            continue;
                        case RenderTableDividerMode.Ignore:
                            continue;
                        default:
                            throw new NotImplementedException();
                    }
                }

                stringBuilder.AppendLine("<tr>");
                foreach (var prop in props)
                {
                    var styles = new List<string>();

                    var value = WebUtility.HtmlEncode(item.GetProperty(prop)?.ToString());
                    if (value?.IsNumericType() is false)
                        styles.Add("text-align: left");

                    var hasBackground = false;
                    if (item.TryGetMetaProperty($"{prop}.background-color", out var backgroundColor))
                    {
                        styles.Add($"background-color: {backgroundColor}");
                        hasBackground = true;
                    }

                    var style = styles.Count > 0 ? $@" style=""{string.Join("; ", styles)}""" : null;

                    value = ReplaceMarkdownBoldWithHtmlBold(value);
                    stringBuilder.AppendLine($"<td{style}{(hasBackground ? "hasBackground" : "")}>{value}</td>");
                }
                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</tbody>");
            stringBuilder.AppendLine("</table>");
            stringBuilder.AppendLine();
#pragma warning restore RCS1197 // Optimize StringBuilder.Append/AppendLine call
        }
    }

    /// <summary>
    /// Wraps the code in a <![CDATA[<pre><code></code></pre>]]> block.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns></returns>
    public static string WrapInCodeBlock(string code)
    {
        return $"""
                <pre>
                <code>
                {code}
                </code>
                </pre>
                """;
    }

    /// <summary>
    /// Replaces the markdown bold (**Bold**) to HTML bold (<![CDATA[<strong>Bold</strong>]]>).
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public static string? ReplaceMarkdownBoldWithHtmlBold(string? text)
    {
        if (text is null)
            return null;

        return MarkdownHelper.GetMarkdownBoldRegex().Replace(text, "<strong>$1</strong>"); //"<b>$1</b>"
    }
}