// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using DataLayer.EfClasses;

namespace ServiceLayer.DatabaseServices.Concrete
{
    public static class SpecialBook
    {
        public static Book CreateSpecialBook()
        {
            var book4 = new Book
            (          
                "Quantum Networking",
                "Entangled quantum networking provides faster-than-light data communications",
                new DateTime(2057, 1, 1),
                "Future Published",
                220,
                null,
                new [] { new Author { Name = "Future Person" } }
            );

            book4.AddReview(new Review { VoterName = "Jon P Smith", NumStars = 5, Comment = "I look forward to reading this book, if I am still alive!" });
            book4.AddReview(new Review { VoterName = "Albert Einstein", NumStars = 5, Comment = "I write this book if I was still alive!" });
            book4.AddPromotion(219, "Save $1 if you order 40 years ahead!");

            return book4;
        }
    }
}