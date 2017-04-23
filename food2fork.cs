using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Foodbot
{
    class food2fork
    {
        public static async Task<ThumbnailCard> getRecipe(string recipe)
        {
            if (recipe.Equals(null))
            {
                return null;
            }

            string userRequest = string.Empty;
            Foodbot.Recipe rawRecipe = await food2fork.getRecipeAsync(recipe);

            if(rawRecipe.Equals(null))
            {
                return null;
            }
            else
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: rawRecipe.image_url));
                List<CardAction> cardButtons = new List<CardAction>();
                CardAction plButton = new CardAction()
                {
                    Value = rawRecipe.source_url,
                    Type = "openUrl",
                    Title = rawRecipe.title
                };
                cardButtons.Add(plButton);
                ThumbnailCard plCard = new ThumbnailCard()
                {
                    Title = rawRecipe.title,
                    Subtitle = rawRecipe.publisher,
                    Images = cardImages,
                    Buttons = cardButtons
                };

                return plCard;
            }
        }

        private static async Task<Foodbot.Recipe> getRecipeAsync(string recipe)
        {
            string rawrecipe = string.Empty;

            string url = $"http://food2fork.com/api/search?key=164ab61bfb52897687578756d3188bc2&q={recipe}";
            string sJson = string.Empty;
            using (WebClient client = new WebClient())
            {
                sJson = await client.DownloadStringTaskAsync(url).ConfigureAwait(false);
            }

            RootObject lookup = null;
            try
            {
                lookup = JsonConvert.DeserializeObject<RootObject>(sJson);
            }
            catch (Exception e)
            {
            }

            if (null != lookup)
            {
                return lookup.recipes[new Random().Next(0, lookup.count + 1)];
            }

            return null;
        }
    }

    public class Recipe
    {
        public string publisher     { get; set; }
        public string f2f_url       { get; set; }
        public string title         { get; set; }
        public string source_url    { get; set; }
        public string recipe_id     { get; set; }
        public string image_url     { get; set; }
        public double social_rank   { get; set; }
        public string publisher_url { get; set; }
    }

    public class RootObject
    {
        public int count            { get; set; }
        public List<Recipe> recipes { get; set; }
    }

    public class RecipeLookup
    {
        public RootObject RootObject { get; set; }
    }
}