using System;

namespace QuizApp.Models
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}