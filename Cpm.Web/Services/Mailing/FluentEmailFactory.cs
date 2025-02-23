using System.Collections.Generic;
using Cpm.Core.Services.Mailing;
using FluentEmail.Core;
using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;

namespace Cpm.Web.Services.Mailing
{
    public class FluentEmailFactory : IFluentEmailFactory
    {
        private readonly IDictionary<EmailCategory, FluentEmailEnvelope> _categoryEnvelopes;
        private readonly ISender _sender;
        private readonly ReplaceRenderer _renderer;

        public FluentEmailFactory(IDictionary<EmailCategory, FluentEmailEnvelope> categoryEnvelopes, ISender sender)
        {
            _categoryEnvelopes = categoryEnvelopes;
            _sender = sender;
            _renderer = new ReplaceRenderer();
        }

        public IFluentEmail Create(EmailCategory category)
        {
            if (!_categoryEnvelopes.ContainsKey(category))
            {
                category = EmailCategory.Default;
            }

            var envelope = _categoryEnvelopes[category];

            return new Email(_renderer, _sender, envelope.FromAddress, envelope.FromName);
        }

        public IFluentEmail Create(EmailCategory category, EmailRecipient recipient)
        {
            return Create(category)
                .To(recipient.Email, recipient.Name);
        }
    }
}