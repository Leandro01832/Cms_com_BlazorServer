using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
        using Newtonsoft.Json;

namespace Models
{
   
        public partial class SearchMetadata
    {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("status")]
            public string? Status { get; set; }

            [JsonProperty("json_endpoint")]
            public Uri? JsonEndpoint { get; set; }

            [JsonProperty("created_at")]
            public string? CreatedAt { get; set; }

            [JsonProperty("processed_at")]
            public string? ProcessedAt { get; set; }

            [JsonProperty("google_url")]
            public Uri? GoogleUrl { get; set; }

            [JsonProperty("raw_html_file")]
            public Uri? RawHtmlFile { get; set; }

            [JsonProperty("total_time_taken")]
            public double TotalTimeTaken { get; set; }
        }

        public partial class Welcome
        {
            [JsonProperty("search_metadata")]
            public SearchMetadata? SearchMetadata { get; set; }

            [JsonProperty("search_parameters")]
            public SearchParameters? SearchParameters { get; set; }

            [JsonProperty("search_information")]
            public SearchInformation? SearchInformation { get; set; }

            [JsonProperty("filters")]
            public Filter[]? Filters { get; set; }

            [JsonProperty("shopping_results")]
            public ShoppingResult[]? ShoppingResults { get; set; }

            [JsonProperty("pagination")]
            public Pagination? Pagination { get; set; }

            [JsonProperty("serpapi_pagination")]
            public Pagination? SerpapiPagination { get; set; }
        }

        public partial class Filter
        {
            [JsonProperty("type")]
            public string? Type { get; set; }

            [JsonProperty("options")]
            public Option[]? Options { get; set; }
        }

        public partial class Option
        {
            [JsonProperty("text")]
            public string? Text { get; set; }

            [JsonProperty("tbs")]
            public string? Tbs { get; set; }
        }

        public partial class Pagination
        {
            [JsonProperty("current")]
            public long Current { get; set; }

            [JsonProperty("next")]
            public Uri? Next { get; set; }

            [JsonProperty("other_pages")]
            public Dictionary<string, Uri>? OtherPages { get; set; }

            [JsonProperty("next_link", NullValueHandling = NullValueHandling.Ignore)]
            public Uri? NextLink { get; set; }
        }

        public partial class SearchInformation
        {
            [JsonProperty("shopping_results_state")]
            public string? ShoppingResultsState { get; set; }

            [JsonProperty("query_displayed")]
            public string? QueryDisplayed { get; set; }

            [JsonProperty("menu_items")]
            public MenuItem[]? MenuItems { get; set; }
        }

        public partial class MenuItem
        {
            [JsonProperty("position")]
            public long Position { get; set; }

            [JsonProperty("title")]
            public string? Title { get; set; }

            [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
            public Uri? Link { get; set; }

            [JsonProperty("serpapi_link", NullValueHandling = NullValueHandling.Ignore)]
            public Uri? SerpapiLink { get; set; }
        }


        public partial class SearchParameters
        {
            [JsonProperty("engine")]
            public string? Engine { get; set; }

            [JsonProperty("q")]
            public string? Q { get; set; }

            [JsonProperty("location_requested")]
            public string? LocationRequested { get; set; }

            [JsonProperty("location_used")]
            public string? LocationUsed { get; set; }

            [JsonProperty("google_domain")]
            public string? GoogleDomain { get; set; }

            [JsonProperty("hl")]
            public string? Hl { get; set; }

            [JsonProperty("gl")]
            public string? Gl { get; set; }

            [JsonProperty("device")]
            public string? Device { get; set; }

            [JsonProperty("tbm")]
            public string? Tbm { get; set; }
        }

        public partial class ShoppingResult
        {
            [JsonProperty("position")]
            public long Position { get; set; }

            [JsonProperty("title")]
            public string? Title { get; set; }

            [JsonProperty("link")]
            public string? Link { get; set; }

            [JsonProperty("product_link")]
            public Uri? ProductLink { get; set; }

            [JsonProperty("product_id")]
            public string? ProductId { get; set; }

            [JsonProperty("serpapi_product_api")]
            public Uri? SerpapiProductApi { get; set; }

            [JsonProperty("source")]
            public string? Source { get; set; }

            [JsonProperty("price")]
            public string? Price { get; set; }

            [JsonProperty("extracted_price")]
            public double ExtractedPrice { get; set; }

            [JsonProperty("rating")]
            public double Rating { get; set; }

            [JsonProperty("reviews")]
            public long Reviews { get; set; }

            [JsonProperty("extensions", NullValueHandling = NullValueHandling.Ignore)]
            public string[]? Extensions { get; set; }

            [JsonProperty("thumbnail")]
            public Uri? Thumbnail { get; set; }

            [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
            public string? Tag { get; set; }

            [JsonProperty("delivery", NullValueHandling = NullValueHandling.Ignore)]
            public string? Delivery { get; set; }
        }
    




}
