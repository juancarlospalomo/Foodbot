using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using System;

namespace Foodbot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {

                RecipeLUIS luis = await LuisFoodBot.ParseUserInput(activity.Text);
                string misunderstand = string.Empty;
                string strRecipe= activity.Text;
                ThumbnailCard recipeCard = null;
                bool prevRequest = false;              

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                StateClient _stateClient = activity.GetStateClient();
                BotData _botData = _stateClient.BotState.GetUserData(activity.ChannelId, activity.Conversation.Id);

                switch (luis.intents[0].intent)
                {
                    case "getRecipe":
                        try
                        {
                            recipeCard = await food2fork.getRecipe(luis.entities[0].entity);
                            prevRequest = true;
                        }catch(Exception e)
                        {
                            misunderstand = "Sorry, I don't understand, perhaps try something like \"I would like Orange Chicken\"";
                            prevRequest = false;
                        }
                        break;
                    case "dislike":
                        strRecipe = _botData.GetProperty<string>("PreviousRequest");
                        if (null == strRecipe)
                        {
                            misunderstand = "I haven't given you a recipe yet!";
                        }
                        else
                        {
                            recipeCard = await food2fork.getRecipe(strRecipe);
                        }
                         break;
                    case "Thanks":
                        misunderstand = "You are very welcome! Happy to help!";
                        break;
                    case "None":
                        misunderstand = "Sorry, I don't understand, perhaps try something like \"I would like Orange Chicken\"";
                        break;
                    default:
                        break;
                }              
                if(misunderstand != string.Empty|| recipeCard == null)
                {
                     Activity reply = activity.CreateReply(misunderstand);

                     await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else
                {
                    Activity replyToConversation = activity.CreateReply(Response(new Random().Next(0, 5)));
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    Attachment plAttachment = recipeCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);

                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                if (prevRequest)
                {
                    _botData.SetProperty<string>("PreviousRequest", luis.entities[0].entity);
                    _stateClient.BotState.SetUserData(activity.ChannelId, activity.Conversation.Id, _botData);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private string Response(int num)
        {
            switch (num)
            {
                case 0:
                    return "How does this sound?";
                case 1:
                    return "This sounds tasty!";
                case 2:
                    return "What do you think of this?";
                case 3:
                    return "This what you're looking for?";
                case 4:
                    return "I found this for you!";
                default:
                    return "How does this sound?";
            }
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            Activity activity = message;
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                IConversationUpdateActivity update = activity;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    if (update.MembersAdded.Any())
                    {
                        var reply = activity.CreateReply();
                        foreach (var newMember in update.MembersAdded)
                        {
                            if (newMember.Id != activity.Recipient.Id)
                            {
                                reply.Text = $"Hello {newMember.Name}! My name is Foodbot, how can I help you today2?";
                            }
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}