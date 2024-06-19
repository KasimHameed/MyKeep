using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyKeep.Entities.TodoList;
using NUnit.Framework.Internal;

namespace MyKeep.Tests;

public class CheckListTests
{
    private Randomizer Randomizer { get; } = new();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void StartCheckListCreatesCheckListAndItem()
    {
        // ARRANGE
        var id = Randomizer.NextGuid();
        var cmd = new StartCheckList(id);
        var key = Randomizer.GetString(8);
        var link = Randomizer.GetString(50);
        var data = new StartCheckListData(link, key);

        // ACT
        var (result, events) = StartCheckListHandler.Handle(cmd, data);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Accepted>());
            Assert.That((result as Accepted)!.Location, Is.EqualTo(link));
            Assert.That(events, Has.Count.EqualTo(2));
            Assert.That(events[0], Is.EqualTo(new CheckListStarted(id)));
            Assert.That(events[1], Is.EqualTo(new CheckListItemAdded(id, key)));
        });
    }

    #region MoveItemToPositionInList Tests

    [Test]
    public void MoveToSameItemProducesNoEvents()
    {
        var id = Randomizer.NextGuid();
        var key = Randomizer.GetString(8);
        var cmd = new MoveItemToPositionInList(id, key, key);
        var entity = new CheckList(id, "", [], []);

        var (result, events) = MoveItemToPositionInListHandler.Handle(cmd, entity);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Ok>());
            Assert.That(events, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MoveTopItemToTopOfListProducesNoEvents()
    {
        var id = Randomizer.NextGuid();
        var key = Randomizer.GetString(8);
        var cmd = new MoveItemToPositionInList(id, key, null);
        var entity = new CheckList(id, "", [new CheckListItem(key, "")], []);

        var (result, events) = MoveItemToPositionInListHandler.Handle(cmd, entity);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Ok>());
            Assert.That(events, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MoveWithMissingKeyResultsIn404AndNoEvents()
    {
        var id = Guid.NewGuid();
        var key1 = Randomizer.GetString(8);
        var cmd = new MoveItemToPositionInList(id, key1, null);
        var entity = new CheckList(id, "", [], []);

        var (result, events) = MoveItemToPositionInListHandler.Handle(cmd, entity);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ProblemHttpResult>());
            Assert.That((result as ProblemHttpResult)!.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(events, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MoveItemAfterNullProducesAcceptedAndMoveToTopEvent()
    {
        var id = Randomizer.NextGuid();
        var key1 = Randomizer.GetString(8);
        var key2 = Randomizer.GetString(8);
        var cmd = new MoveItemToPositionInList(id, key2, null);
        var entity = new CheckList(id, "", [new CheckListItem(key1, ""), new CheckListItem(key2, "")], []);

        var (result, events) = MoveItemToPositionInListHandler.Handle(cmd, entity);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Accepted>());
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.EqualTo(new ItemMovedToTopOfList(id, key2)));
        });
    }

    [Test]
    public void MissingAfterKeyProduces400AndNoEvents()
    {
        var id = Randomizer.NextGuid();
        var key1 = Randomizer.GetString(8);
        var key2 = Randomizer.GetString(8);
        var cmd = new MoveItemToPositionInList(id, key1, key2);
        var entity = new CheckList(id, "", [new CheckListItem(key1, "")], []);

        var (result, events) = MoveItemToPositionInListHandler.Handle(cmd, entity);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ProblemHttpResult>());
            Assert.That((result as ProblemHttpResult)!.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(events, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void MoveItemAfterItemProducesAcceptedAndMovedEvent()
    {
        var id = Randomizer.NextGuid();
        var key = Randomizer.GetString(8);
        var afterKey = Randomizer.GetString(8);
        var cmd = new MoveItemToPositionInList(id, key, afterKey);
        var entity = new CheckList(id, "", [new CheckListItem(key, ""), new CheckListItem(afterKey, "")], []);

        var (result, events) = MoveItemToPositionInListHandler.Handle(cmd, entity);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Accepted>());
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.EqualTo(new ItemMovedAfterItemInList(id, key, afterKey)));
        });
    }

    #endregion
    
    #region UpdateCheckListItem Tests

    [Test]
    public void UpdatePendingCheckListItemWithSameTextProducesOkAndNoEvents()
    {
        var id = Randomizer.NextGuid();
        var key = Randomizer.GetString(8);
        var text = Randomizer.GetString(20);
        const bool completed = false;
        var cmd = new UpdateCheckListItem(id, key, text, completed);
        var entity = new CheckList(id, "", [new CheckListItem(key, text)], []);

        var (result, events) = UpdateCheckListItemHandler.Handle(cmd, entity);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Ok>());
            Assert.That(events, Has.Count.EqualTo(0));
        });
    }
    
    [Test]
    public void UpdateCompletedCheckListItemWithSameTextProducesOkAndNoEvents()
    {
        var id = Randomizer.NextGuid();
        var key = Randomizer.GetString(8);
        var text = Randomizer.GetString(20);
        const bool completed = true;
        var cmd = new UpdateCheckListItem(id, key, text, completed);
        var entity = new CheckList(id, "", [], [new CheckListItem(key, text)]);

        var (result, events) = UpdateCheckListItemHandler.Handle(cmd, entity);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Ok>());
            Assert.That(events, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void UpdatePendingListItemWithNewTextReturnsAcceptedAndEvent()
    {
        var id = Randomizer.NextGuid();
        var key = Randomizer.GetString(8);
        var originalText = Randomizer.GetString(20);
        var newText = Randomizer.GetString(20);
        const bool completed = false;
        var cmd = new UpdateCheckListItem(id, key, newText, completed);
        var entity = new CheckList(id, "", [new CheckListItem(key, originalText)], []);

        var (result, events) = UpdateCheckListItemHandler.Handle(cmd, entity);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Accepted>());
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.EqualTo(new PendingItemTextUpdated(id, newText, key)));
        });
    }
    
    #endregion
}