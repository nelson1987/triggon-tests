using MongoDB.Bson;

namespace Triggon.Core.Entities.Bases;
public abstract class EntityBase
{
    public Guid Id { get; set; }
}

public abstract class MongoEntiyBase
{
    public ObjectId Id { get; set; }
}