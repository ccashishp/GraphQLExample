using System;

namespace Orders.Models
{
    public class Order
    {
        public Order(string id, string name, string description, DateTime createdDate, int customerId)
        {
            Id = id;
            Name = name;
            Description = description;
            CreatedDate = createdDate;
            CustomerId = customerId;
            Status = OrderStatuses.CREATED;
        }

        public string Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; private set; }
        public int CustomerId { get; set; }
        public OrderStatuses Status { get; private set; }

        public void Start()
        {
            Status = OrderStatuses.PROCESSING;
        }
    }

    [Flags] // useful for subscriptions
    public enum OrderStatuses
    {
        CREATED = 2,
        PROCESSING = 4,
        COMPLETED = 8,
        CANCELED = 16,
        CLOSED = 32
    }
}
