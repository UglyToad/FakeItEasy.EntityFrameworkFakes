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


## Implementation ##

This library adapts [this implementation of Entity Framework mocking][link1].

[link0]: https://github.com/FakeItEasy/FakeItEasy
[link1]: https://msdn.microsoft.com/en-gb/data/dn314429.aspx