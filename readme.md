# Entity Framework Fakes #

This library provides convenience faking methods for Entity Framework 6 using [FakeItEasy][link0].

## Usage ##

Assuming you have some kind of DbContext:

	public class MyContext : DbContext
	{
		// My DbSets...
	}

You can get a faked instance of the context with:

	var context = ContextFaker.CreateContext<MyContext>();

The DbSets can then be configured using:

	// ContextFaker.ContextReturnsDbSet(lambda expression accessing DbSet, list of test data)
	ContextFaker.ContextReturnsDbSet(() => context.Acorns, new List<Acorn>());

If you use ```DbSet.Find()``` on your entity the ContextFaker will attempt to use properties with the name "Id" or "ClassName" + "Id". If you need to configure a property with a different name, use the following method:

	ContextFaker.AddIdGetterForType((T entity) => entity.SomeWeirdIdProperty); 

## Notes ##

The faked set supports the following operations:

+ Add
+ AddRange
+ Attach
+ Find
+ FindAsync
+ Remove
+ RemoveRange

Any queries using ```Include()``` will effectively ignore the Include and just use the data passed in to the creation of the fake DbSet.

Be aware that any data passed into ```ContextReturnsDbSet()``` will be changed by operations against the DbSet so ensure unexpected changes to the list do not affect your tests.


## Implementation ##

This library adapts [this implementation of Entity Framework mocking][link1].

[link0]: https://github.com/FakeItEasy/FakeItEasy
[link1]: https://msdn.microsoft.com/en-gb/data/dn314429.aspx