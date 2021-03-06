﻿using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using FNLTB.Json;

namespace FNLTB.Commands
{
    class EmojiList
    {
        public static string[] Aliases =
        {
            "emojilist",
            "emoji-list",
            "emoji",
            "emojis",
            "emotelist",
            "emote-list",
            "emote",
            "emotes"
        };
        public static string Description = "Shows a categorized list of avaliable emojis.";
        public static string[] Usages = { "emojilist [category]" };
        public static string Permission = "Anyone can us it.";

        public async static void DoCommand()
        {
            await Program.Log();
            var message = Recieved.Message;
            string[] m = message.Content.Split();
            var r = new Random();

            var embed = new EmbedBuilder()
                .WithColor(new Color(0xFFDD00))
                .WithFooter("Made by ExAtom");

            if (m.Length > 2)
                await message.Channel.SendMessageAsync("❌ Too many parameters!");
            else if (m.Length == 1)
            {
                embed.WithAuthor(author => { author.WithName("Avaliable emojis"); });
                foreach (var i in Program._client.Guilds.Where(x => x.Name.Contains("Emojis") && x.OwnerId == BaseConfig.GetConfig().OwnerID).OrderBy(x => x.Name))
                {
                    string emojis = "";
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 4; k++)
                            emojis += $"{i.Emotes.ElementAt(r.Next(0, i.Emotes.Count()))} ";
                        emojis += "\n";
                    }
                    embed.AddField(i.Name, emojis, true);
                }
                var settings = Settings.PullData();
                embed.WithDescription($"For more use `{settings[settings.IndexOf(settings.Find(x => x.ServerID == ((SocketGuildChannel)message.Channel).Guild.Id))].Prefix}emojilist <category>`");
            }
            else
            {
                SocketGuild server;
                try
                {
                    server = Program._client.Guilds.First(x => x.Name.ToLower() == m[1].ToLower());
                    embed.WithAuthor(author => { author.WithName(server.Name); });
                }
                catch (Exception) { return; }
                if (server == null)
                    return;
                var emotes = server.Emotes.Where(x => !x.Animated).OrderBy(x => x.Name);
                var animatedEmotes = server.Emotes.Where(x => x.Animated).OrderBy(x => x.Name);

                for (int i = 0; i < Math.Ceiling(emotes.Count() / 25.0); i++)
                {
                    string output = "";
                    int counter = 0;
                    int emoteCount = 0;
                    foreach (var j in emotes.Skip(i * 25))
                    {
                        emoteCount++;
                        counter++;
                        output += counter % 5 == 0 && counter != 0 ? $"{j}\n" : j.ToString();
                        if (emoteCount >= 25)
                            break;
                    }
                    embed.AddField("Emotes", output, true);
                }
                for (int i = 0; i < Math.Ceiling(animatedEmotes.Count() / 25.0); i++)
                {
                    string output = "";
                    int counter = 0;
                    int emoteCount = 0;
                    foreach (var j in animatedEmotes.Skip(i * 25))
                    {
                        emoteCount++;
                        counter++;
                        output += counter % 5 == 0 && counter != 0 ? $"{j}\n" : j.ToString();
                        if (emoteCount >= 25)
                            break;
                    }
                    embed.AddField("Animated", output, true);
                }
                embed.WithDescription($"[Server Invite]({server.GetInvitesAsync().Result.FirstOrDefault().Url})");
            }

            try { await message.Channel.SendMessageAsync(null, embed: embed.Build()); }
            catch (Exception) { }
        }
    }
}
