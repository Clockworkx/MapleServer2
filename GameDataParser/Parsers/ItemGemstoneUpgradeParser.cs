﻿using System.Text.RegularExpressions;
using System.Xml;
using GameDataParser.Files;
using GameDataParser.Files.MetadataExporter;
using GameDataParser.Parsers.Helpers;
using Maple2.File.IO.Crypto.Common;
using Maple2Storage.Types;
using Maple2Storage.Types.Metadata;

namespace GameDataParser.Parsers;

public class ItemGemstoneUpgradeParser : Exporter<List<ItemGemstoneUpgradeMetadata>>
{
    public ItemGemstoneUpgradeParser(MetadataResources resources) : base(resources, MetadataName.ItemGemstoneUpgrade) { }

    protected override List<ItemGemstoneUpgradeMetadata> Parse()
    {
        List<ItemGemstoneUpgradeMetadata> gems = new();
        foreach (PackFileEntry entry in Resources.XmlReader.Files)
        {
            if (!entry.Name.StartsWith("table/na/itemgemstoneupgrade"))
            {
                continue;
            }

            // Parse XML
            XmlDocument document = Resources.XmlReader.GetXmlDocument(entry);
            XmlNodeList? keys = document.SelectNodes("/ms2/key");
            if (keys is null)
            {
                continue;
            }

            foreach (XmlNode key in keys)
            {
                if (ParserHelper.CheckForNull(key, "ItemId", "GemLevel"))
                {
                    continue;
                }

                ItemGemstoneUpgradeMetadata metadata = new()
                {
                    ItemId = int.Parse(key.Attributes!["ItemId"]!.Value),
                    GemLevel = byte.Parse(key.Attributes["GemLevel"]!.Value)
                };
                _ = int.TryParse(key.Attributes["NextItemID"]?.Value ?? "0", out metadata.NextItemId);

                for (int i = 1; i < 5; i++)
                {
                    if (key.Attributes["IngredientItemID" + i] is null)
                    {
                        continue;
                    }

                    metadata.IngredientItems.Add(Regex.Match(key.Attributes["IngredientItemID" + i]!.Value, @"[a-zA-Z]+").Value);
                    metadata.IngredientAmounts.Add(int.Parse(key.Attributes["IngredientCount" + i]!.Value));
                }

                gems.Add(metadata);
            }
        }

        return gems;
    }
}
