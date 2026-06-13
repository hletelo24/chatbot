using System;
using System.Collections.Generic;
using System.Text;

namespace chatbot
{
    class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }
}
