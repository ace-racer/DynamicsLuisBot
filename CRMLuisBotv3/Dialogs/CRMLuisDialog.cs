using CRMLuisBotv3.Dynamics;
using CRMLuisBotv3.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CRMLuisBotv3.Dialogs
{
    [LuisModel("20006101-5c9b-4a1a-9d94-b0b897c3c4f5", "87452bafe2bd46d29b5b721525409212")]
    [Serializable]
    public class CRMLuisDialog : LuisDialog<object>
    {
        private const string EntityPhoneNumber = "builtin.phonenumber";


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Hello there welcome to {ApplicationConstants.OrganizationName}! Please provide your phone number to proceed!";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Get Phone Number")]
        public async Task GetCasesForContact(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to {ApplicationConstants.OrganizationName}! We are analyzing your message: '{message.Text}'...");
            EntityRecommendation phoneNumberEntityRecommendation;
            if(result.TryFindEntity(EntityPhoneNumber, out phoneNumberEntityRecommendation))
            {
                var phoneNumber = phoneNumberEntityRecommendation.Entity;
                await context.PostAsync($"Looking up the user with phone number '{phoneNumber}' in our system");
                if(!string.IsNullOrWhiteSpace(phoneNumber))
                {
                   CrmDetail crmDetail = null;

                    // get details of contact with phone number in CRM
                    if(CRMHelpers.TryGetDetailsFromDynamics(phoneNumber, out crmDetail))
                    {
                        if (crmDetail != null)
                        {
                            // var contactName = contactEntity.GetAttributeValue<string>("fullname");
                            await context.PostAsync($"Welcome {crmDetail.ContactDetails.FullName}! Please wait as we search for your cases registered with us...");
                            var resultMessage = context.MakeMessage();
                            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            resultMessage.Attachments = new List<Attachment>();

                            foreach (var caseDetail in crmDetail.ContactCaseDetails)
                            {
                                ThumbnailCard thumbnailCard = new ThumbnailCard()
                                {
                                    Title = caseDetail.CaseTitle,
                                    Text = caseDetail.CaseDescription                                                                       
                                };

                                resultMessage.Attachments.Add(thumbnailCard.ToAttachment());
                            }

                            await context.PostAsync(resultMessage);
                            context.Wait(this.MessageReceived);                          
                        }
                    }
                    else
                    {
                        // if contact does not exist then show message
                        await context.PostAsync($"Hmm...we could not find your phone number in our systems, can you give your phone number again?");
                    }                    
                }
            }
            else
            {
                await context.PostAsync($"Hmm...we could not find your phone number in the message. Can you please type your phone number?");
            }
          
        }

    }
}