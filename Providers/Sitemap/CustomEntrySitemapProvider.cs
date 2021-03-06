﻿using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using IDeliverable.Seo.Models;
using IDeliverable.Seo.Services;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace IDeliverable.Seo.Providers.Sitemap
{
    [OrchardFeature("IDeliverable.Seo.Sitemap")]
    public class CustomEntrySitemapProvider : SitemapProviderBase
    {
        private readonly UrlHelper _urlHelper;
        private readonly IRepository<CustomSitemapEntryRecord> _customSitemapEntryRepository;

        public CustomEntrySitemapProvider(UrlHelper urlHelper, IRepository<CustomSitemapEntryRecord> customSitemapEntryRepository, ISitemapEntryHandler sitemapEntryHandlers) : base(sitemapEntryHandlers)
        {
            _urlHelper = urlHelper;
            _customSitemapEntryRepository = customSitemapEntryRepository;
        }

        public override string Name => "CustomEntry";
        public override LocalizedString DisplayName => T("Custom Entry");

        public override void GetSitemapEntries(SitemapContext context)
        {
            var records = _customSitemapEntryRepository.Table.ToArray();

            foreach (var record in records)
            {
                var entry = CreateEntry(
                    record,
                    record.Url.StartsWith("~") || record.Url.StartsWith("/") ? _urlHelper.Content(record.Url) : record.Url,
                    record.LastModifiedUtc,
                    record.ChangeFrequency,
                    record.Priority);

                entry.Context = record.Id.ToString();
                context.Entries.Add(entry);
            }
        }

        public override void GetSitemapEntryMetadata(SitemapEntryMetadataContext context)
        {
            if (context.Entry.ProviderName != Name)
                return;

            var contentId = XmlHelper.Parse<int>(context.Entry.Context);
            context.Metadata.EditRouteValues = new RouteValueDictionary(new
            {
                Area = "IDeliverable.Seo",
                Controller = "CustomSitemapEntry",
                Action = "Edit",
                Id = contentId
            });
        }
    }
}