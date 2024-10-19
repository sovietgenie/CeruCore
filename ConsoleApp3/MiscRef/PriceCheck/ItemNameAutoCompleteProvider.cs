//using DSharpPlus.Entities;
//using DSharpPlus.SlashCommands;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CeruCore.MiscRef.PriceCheck
//{
//    internal class ItemNameAutoCompleteProvider : IAutocompleteProvider
//    { 
//        public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
//        {
//            var tags = tagService
//             .GetTags()
//             .Where(x => x.Name.StartsWith(ctx.UserInput, StringComparison.OrdinalIgnoreCase))
//             .ToDictionary(x => x.Name, x => x.Id);

//            return ValueTask.FromResult(tags);
//        }
//    }
//}
