﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using segundoparcial_mtorres.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace segundoparcial_mtorres.DataLayer.Mappers
{
    public class MangaMapper : IEntityTypeConfiguration<Manga>
    {
        public void Configure(EntityTypeBuilder<Manga> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(p => p.ImageURL)
                .IsUnicode(false)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            builder.HasData(new List<Manga>
            {
                new Manga(1, 1, "Attack on Titan","Hajime Isayama","A century ago, the grotesque giants known as Titans appeared and consumed all but a few thousand humans. The survivors took refuge behind giant walls. Today, the threat of the Titans is a distant memory, and a boy named Eren yearns to explore the world beyond Wall Maria. But what began as a childish dream will become an all-too-real nightmare when the Titans return and humanity is once again on the brink of extinction", 129, false, "https://i1.wp.com/lacomikeria.com/wp-content/uploads/2020/06/Attack-on-Titan-manga.jpg?resize=600%2C400&ssl=1"),
                new Manga(2, 5, "Lovely Complex", "Aya Nakahara", "Love is unusual for Koizumi Risa and Ootani Atsushi, who are both striving to find their ideal partner in high school—172 cm tall Koizumi is much taller than the average girl, and Ootani is much shorter than the average guy at 156 cm. To add to their plights, their crushes fall in love with each other, leaving Koizumi and Ootani comically flustered and heartbroken. To make matters worse, they\'re even labeled as a comedy duo by their homeroom teacher due to their personalities and the stark difference in their heights, and their classmates even think of their arguments as sketches.", 66, false, "https://uploads.spiritfanfiction.com/fanfics/historias/202001/because-there-is-you-18417494-260120201649.png"),
                new Manga(3, 4, "Goodnight, Punpun", "Inio Asano", "A coming-of-age drama story, it follows the life of a child named Onodera Punpun, from his elementary school years to his early 20s, as he copes with his dysfunctional family, love life, friends, life goals and hyperactive mind, while occasionally focusing on the lives and struggles of his schoolmates and family. Punpun and the members of his family are normal humans, but are depicted to the reader in the forms of birds. The manga explores themes such as depression, love, social isolation, sex, death, and family.", 147, true, "https://media.metrolatam.com/2020/01/22/template90-703d63d7f00a258de6aea0cc70cd8d85-600x400.jpg"),
            });
        }
    }
}