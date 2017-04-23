using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foodbot
{
    public class ResponseFactory
    {
        public static string getResponse()
        {
            int num = new Random().Next(0, 5);
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

    }
}