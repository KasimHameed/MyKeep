using Marten.Events.Aggregation;

namespace MyKeep.Entities.TodoList;

public record CheckList(Guid Id, string Title, CheckListItem[] PendingItems, CheckListItem[] CompletedItems, string Color);

public record CheckListItem(string Key, string Text);

public class CheckListProjection: SingleStreamProjection<CheckList>
{
    public CheckListProjection()
    {
        DeleteEvent<ListDeleted>();
    }
    
    private static int IndexOf(IEnumerable<CheckListItem> items, string key)
        => items.Zip(Enumerable.Range(0, int.MaxValue), Tuple.Create).Single(i => i.Item1.Key == key).Item2;
    
    public static CheckList Create(CheckListStarted evt) => new(evt.Id, "", [], [], "lightgray");
    
    public static CheckList Create(CheckListStartedWithColor evt) => new(evt.Id, "", [], [], evt.Color ?? "lightgray");

    public static CheckList Apply(CheckListItemAdded evt, CheckList entity) =>
        entity with { PendingItems = [..entity.PendingItems, new CheckListItem(evt.Key, "")]};

    public static CheckList Apply(PendingItemTextUpdated evt, CheckList entity)
    {
        var index = IndexOf(entity.PendingItems, evt.Key);
        return entity with
        {
            PendingItems =
            [
                ..entity.PendingItems[..index],
                entity.PendingItems[index] with { Text = evt.Text },
                ..entity.PendingItems[(index + 1)..]
            ]
        };
    }


    public static CheckList Apply(CompletedItemTextUpdated evt, CheckList entity) 
    {
        var index = IndexOf(entity.CompletedItems, evt.Key);
        return entity with
        {
            CompletedItems =
            [
                ..entity.CompletedItems[..index],
                entity.CompletedItems[index] with { Text = evt.Text },
                ..entity.CompletedItems[(index + 1)..]
            ]
        };
    }

    public static CheckList Apply(ItemMovedToTopOfList evt, CheckList entity)
    {
        var fromIndex = IndexOf(entity.PendingItems, evt.Key);
        return entity with
        {
            PendingItems =
            [
                entity.PendingItems[fromIndex],
                ..entity.PendingItems[..fromIndex],
                ..entity.PendingItems[(fromIndex+1)..]
            ]
        };
    }

    public static CheckList Apply(ItemMovedAfterItemInList evt, CheckList entity)
    {
        var fromIndex = IndexOf(entity.PendingItems, evt.Key);
        var toIndex = IndexOf(entity.PendingItems, evt.AfterKey);
        
        if (fromIndex < toIndex)
        {
            return entity with
            {
                PendingItems =
                [
                    ..entity.PendingItems[..fromIndex],
                    ..entity.PendingItems[(fromIndex + 1)..(toIndex + 1)],
                    entity.PendingItems[fromIndex],
                    ..entity.PendingItems[(toIndex + 1)..]
                ]
            };
        } 

        return entity with
        {
            PendingItems =
            [
                ..entity.PendingItems[..(toIndex + 1)],
                entity.PendingItems[fromIndex],
                ..entity.PendingItems[(toIndex + 1)..fromIndex],
                ..entity.PendingItems[(fromIndex + 1)..]
            ]
        };
    }

    public static CheckList Apply(ItemMarkedCompleted evt, CheckList entity)
    {
        var index = IndexOf(entity.PendingItems, evt.Key);
        return entity with
        {
            PendingItems = [..entity.PendingItems[..index], ..entity.PendingItems[(index + 1)..]],
            CompletedItems = [..entity.CompletedItems, entity.PendingItems[index]]
        };
    }

    public static CheckList Apply(ItemReactivated evt, CheckList entity)
    {
        var index = IndexOf(entity.CompletedItems, evt.Key);
        return entity with
        {
            CompletedItems = [..entity.CompletedItems[..index], ..entity.CompletedItems[(index + 1)..]],
            PendingItems = [..entity.PendingItems, entity.CompletedItems[index]]
        };
    }

    public static CheckList Apply(CheckListRenamed evt, CheckList entity) => entity with { Title = evt.Title };

    public static CheckList Apply(PendingItemDeleted evt, CheckList entity)
    {
        var index = IndexOf(entity.PendingItems, evt.Key);
        return entity with
        {
            PendingItems = [..entity.PendingItems[..index], ..entity.PendingItems[(index + 1)..]]
        };
    }
    
    public static CheckList Apply(CompletedItemDeleted evt, CheckList entity) 
    {
        var index = IndexOf(entity.CompletedItems, evt.Key);
        return entity with
        {
            CompletedItems = [..entity.CompletedItems[..index], ..entity.CompletedItems[(index + 1)..]]
        };
    }

    public static CheckList Apply(CheckListColorUpdated evt, CheckList entity) => entity with { Color = evt.Color };
}

