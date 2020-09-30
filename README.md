# First Attempt at a Blazor WebAssembly 

The solution includes unit tests for all functionality, though I've skipped the data classes.

Still to do are the unit tests for the front end, made simpler by the fact that it uses injection (I like that)!

There is a unit test for the Database inserts, essential I feel as no ORM was used so we can't work under the assumption that that was tested.

SQL scripts are incldued in the AdoRepository project for database and table creation (and used in the testing code). It's been a long while since I used ADO rather than an ORM, probably need to take another look at this area, though I have at least made it relatively simple to swap out by using the repository pattern. Unit of work was overkill for this, but I could see that coming in as things got more complicated.

Talking of databases, the email column has a unique value constraint on it, but I have not made this the primary key; the reason for this is that people can change email addresses but they are still the same person, we wouldn't want to force them to create a new account, or to have to do this in the background for them.

There are still some improvements that could be made to the front end, particularly in terms of validation handling, e.g. updating on input rather than lost focus, checking all rules at once. Also, the messages persist, I might look at adding a timer to remove them after a few seconds, or make them user closable. Of course, on success I would expect to navigate to another page rather than displaying a message to confirm this result. I think that it would be a start to take to get feedback on at least.

Impressions on Blazor... I like it, but then I've been using C# for quite some time. I would say though, that having spent the last 6 months or so teaching myself Angular, it seels a bit strange to see code and HTML in the same file (yes I know you can stick template HTML into the Angular components, but I never have). I might get used to this, or I might prefer to move the code behind, which is perfectly possible with Blazor.