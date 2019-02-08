
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Checkpoint3
{
    class Program
    {
        static void Main(string[] args)
        {
            //create the context
            Context todoContext = new Context();
            //keep this here while working on the program, this will allow a new db structure to start fresh each time we run a test
            // todoContext.Database.EnsureDeleted();
            //create the table
            todoContext.Database.EnsureCreated();

            //begin user interaction
            System.Console.WriteLine("-----------------------");
            System.Console.WriteLine("Welcome to My ToDo App!");
            System.Console.WriteLine("-----------------------");

            //the initial state of the program is to list currently incomplete todos, this will list nothing first time the app runs
            Utils.ListIncomplete(todoContext);

            //then give a menu of commands
            Utils.OptionsMenu();

            bool needsCommand = true; 
            while (needsCommand)
            {
                //read the user's input 
                string input = Console.ReadLine(); 

                //Parse the input into an int if possible
                bool successfullyParsed = int.TryParse(input, out int parsedInput);

                //send tp proper method depending on user input
                if (parsedInput == 1)
                {
                    Utils.AddToDo(todoContext);
                }
                else if (parsedInput == 2)
                {
                        Utils.UpdateToDo(todoContext);
                }
                else if (parsedInput == 3)
                {
                    Utils.MarkComplete(todoContext);
                }
                else if (parsedInput == 4)
                {
                    Utils.Delete(todoContext);
                }
                else if (parsedInput == 5)
                {
                    Utils.ListComplete(todoContext);
                }
                else if (parsedInput == 6)
                {
                    Utils.ListAll(todoContext);
                }
                else if (parsedInput == 7)
                {
                    Utils.ListType(todoContext, 7);
                }
                else if (parsedInput == 8)
                {
                    Utils.ListType(todoContext, 8);
                }
                else if (parsedInput == 9)
                {
                    Utils.ListType(todoContext, 9);
                }
                else if (parsedInput == 10)
                {
                    System.Console.WriteLine("Thank you for using My ToDo App. See you next time!");
                    needsCommand = false;
                }
                else if (input.ToLower() == "help")
                {
                    Utils.OptionsMenu();
                }
                else
                {
                    System.Console.WriteLine("Here's your current ToDo list!");
                    Utils.ListIncomplete(todoContext);
                }
            }            
        }
    }

    //default position for an new item will be general. A good future extension project would be to explore how a user could create their own type
    public enum Type {general, personal, work, school}
    //default position for a new item will be incomplete
    public enum Status {incomplete, complete}

    //begin item class
    public class Item 
    {
        //Item properties
        public int id {get;set;}
        public int displayID {get;set;}
        public string description {get;set;}
        public Type type {get; set;}
        public Status status {get; set;}
        public DateTime deadline {get; set;}

        //need an empty constructor to work with Entity Framework
        public Item(){}
        
        //the only property required to instantiate a new todo is the description
        public Item(string description)
        {
            this.description = description;
        }
    }

    //begin utils class, put methods all methods required to perform app tasks here, going to be static methods
    public static class Utils
    {        
        //method ListIncomplete takes in todoContext from main and prints out any todos that need completion, this is the default state for the program
        public static void ListIncomplete(Context todoContext)
        {
            var results = from t in todoContext.todos where (t.status == Status.incomplete)
            select t;
            
            //Will display the following message if there are no incomplete todos
            if (results.Count() == 0)
            {
                System.Console.WriteLine("You currently have nothing ToDo.");
                System.Console.WriteLine("");
            }
            //if there are incomplete todos, list them
            else
            {
                System.Console.WriteLine("-----------------");
                System.Console.WriteLine("My Current ToDos:");
                System.Console.WriteLine("-----------------");

                //call function to print the list based on the results produced above
                WriteList(results);
                System.Console.WriteLine("");
            }

            //display ids need to be saved to the database so that the user can use them to identify the item the'd like to manipulate
            todoContext.SaveChanges();
        }
        //method OptionsMenu simply prints the menu of options for user to select from
        public static void OptionsMenu()
        {
            System.Console.WriteLine("-------");
            System.Console.WriteLine("Actions");
            System.Console.WriteLine("-------");
            System.Console.WriteLine("Enter 1 to ADD a new ToDo");
            System.Console.WriteLine("Enter 2 to UPDATE an existing ToDo");
            System.Console.WriteLine("Enter 3 to MARK a ToDo COMPLETE");
            System.Console.WriteLine("Enter 4 to DELETE an existing ToDo");
            System.Console.WriteLine("");
            System.Console.WriteLine("----------");
            System.Console.WriteLine("List Views");
            System.Console.WriteLine("----------");
            System.Console.WriteLine("Enter 5 to view COMPLETED ToDos ONLY");
            System.Console.WriteLine("Enter 6 to view ALL ToDos on record");
            System.Console.WriteLine("Enter 7 to view PERSONAL ToDos ONLY");
            System.Console.WriteLine("Enter 8 to view WORK ToDos ONLY");
            System.Console.WriteLine("Enter 9 to view SCHOOL ToDos ONLY");
            System.Console.WriteLine("");
            System.Console.WriteLine("-------------");
            System.Console.WriteLine("Exit Strategy");
            System.Console.WriteLine("-------------");            
            System.Console.WriteLine("Enter 10 to QUIT ");
            System.Console.WriteLine("");
            System.Console.WriteLine("----");
            System.Console.WriteLine("Help");
            System.Console.WriteLine("----");            
            System.Console.WriteLine(@"Enter 'help' to get HELP");
            System.Console.WriteLine("");
            System.Console.WriteLine("----------------");
            System.Console.WriteLine("My Current ToDos");
            System.Console.WriteLine("----------------");            
            System.Console.WriteLine("Enter ANY OTHER KEY to view your currently incomplete ToDos!");
        }
        //FUNCTION 1: AddToDo takes in todo context and uses this context to add a new to do to the list
        public static void AddToDo(Context todoContext)
        {
            //take input for new ToDo description
            System.Console.WriteLine("Please enter your new ToDo:");
            string description = Console.ReadLine();

            //initialize a new item with a description
            Item newItem = new Item(description);
            todoContext.todos.Add(newItem);
            
            //Prompt user for a type for this todo + set type
            System.Console.WriteLine("What type of ToDo is this?");
            SetType(newItem);

            //Prompt user for a deadline for this todo + set type
            System.Console.WriteLine("When is the deadline for this ToDo?");
            SetDeadline(newItem);

            System.Console.WriteLine("Your ToDo has been added. Thank you!");
            System.Console.WriteLine("");

            //save changes to store this new ToDo and it's attributes
            todoContext.SaveChanges();
        }
        //method SetType takes in an item and updates it's type property
        public static void SetType(Item item)
        {
            bool loop = true;
            while (loop)
            {
                //give type options
                System.Console.WriteLine("Please enter 1 for Personal, 2 for Work, 3 for School, or 4 for general.");
                
                //read the user's input 
                string input = Console.ReadLine(); 

                //Parse the input into an int if possible
                bool successfullyParsed = int.TryParse(input, out int parsedInput);
                //begin conditional statements to set the type according to the number entered
                if (parsedInput == 1)
                {
                    item.type = Type.personal;
                    loop = false;
                }
                else if (parsedInput == 2)
                {
                    item.type = Type.work;
                    loop = false;
                }
                else if (parsedInput == 3)
                {
                    item.type = Type.school;
                    loop = false;
                }
                else if (parsedInput == 4)
                {
                    item.type = Type.general;
                    loop = false;
                }
                else 
                {
                    Console.WriteLine("Not a valid entry. Please try again.");
                } 
            }
        }
        //method SetDeadline takes in an item and sets it's deadline property
        public static void SetDeadline(Item item)
        {
            //prompt user for a deadline
            System.Console.WriteLine("Please enter the deadline for this ToDo. Follow this Date/Time format: MM/DD/YYYY HH:MM:SS AM/PM.");
            System.Console.WriteLine("Don't want to set a deadline? No problem. Press any key to continue with no deadline.");

            //read the user's input 
            string input = Console.ReadLine(); 

            //Parse the input into a datetime if possible
            bool successfullyParsed = DateTime.TryParse(input, out DateTime dt);
            if (successfullyParsed)
            {
                //set the item's deadline to parsed DateTime input
                item.deadline = dt;
            }
        }

        //FUNCTION 2: UpdateToDo takes in the todoContext and makes changes to a specified todo's description, type or deadline
        public static void UpdateToDo(Context todoContext)
        { 
            //begin loop to confirm and update the ToDo
            bool loop = true;
            while (loop)
            {
                //prompt for displayID
                System.Console.WriteLine("Please enter the ID of the ToDo item you would like to update:");

                //read the user's input 
                string input = Console.ReadLine(); 

                //Parse the input into an int if possible
                bool successfullyParsed = int.TryParse(input, out int parsedInput);

                var match = from t in todoContext.todos where t.displayID == parsedInput 
                select t;

                //if one unique match is returned, update this item
                if (match.Count() == 1)
                {
                    foreach (Item item in match)
                    {
                        System.Console.WriteLine($"Thanks. Here's the current description of this ToDo: {item.description}");
                        System.Console.WriteLine("Please enter your new description below OR enter 0 to keep this description.");
                        string newDescription = Console.ReadLine();
                        if (newDescription != "0")
                        {
                            item.description = newDescription;
                        }
                        System.Console.WriteLine("Thanks. Would you like to update the type? Enter any key to update OR enter 0 to keep the current type.");
                        string newType = Console.ReadLine().ToLower();
                        if (newType != "0")
                        {
                            SetType(item);
                        }
                        System.Console.WriteLine("Thanks. Would you like to update the deadline? Enter any key to update OR enter 0 to keep the current deadline.");
                        string newDeadline = Console.ReadLine().ToLower();
                        if (newDeadline != "0")
                        {
                            SetDeadline(item);
                        }
                        System.Console.WriteLine("Your ToDo has been updated. Thank you!");
                        System.Console.WriteLine("");
                        //exit loop
                        loop = false;
                    }
                }

                //If the user has run multiple lists recently, there can be multiple equivalent display ids. 
                //In that case, guide the user to match Item by description
                else if (match.Count() > 1)
                {
                    System.Console.WriteLine("There are multiple ToDos with that ID. Sorry about that! Please enter the description of the item you'd like to mark complete.");
                    string descriptionMatch = Console.ReadLine().ToLower().Trim();
                    //tracking if a match is found. In case it's not, I want to display some feedback to the user.
                    bool matchFound = false;

                    foreach (Item item in match)
                    {
                        //if match complete the task and move on
                        if (descriptionMatch == item.description.ToLower().Trim())
                        {
                            System.Console.WriteLine("Great, let's update it. Please enter your new description below OR enter 0 to keep the current description.");
                            string newDescription = Console.ReadLine();
                            if (newDescription != "0")
                            {
                                item.description = newDescription;
                            }
                            System.Console.WriteLine("Thanks. Would you like to update the type? Enter any key to update OR enter 0 to keep the current type.");
                            string newType = Console.ReadLine().ToLower();
                            if (newType != "0")
                            {
                                SetType(item);
                            }
                            System.Console.WriteLine("Thanks. Would you like to update the deadline? Enter any key to update OR enter 0 to keep the current deadline.");
                            string newDeadline = Console.ReadLine().ToLower();
                            if (newDeadline != "0")
                            {
                                SetDeadline(item);
                            }
                            System.Console.WriteLine("Your ToDo has been updated. Thank you!");
                            System.Console.WriteLine("");
                            matchFound = true;
                            loop = false;
                            break;

                        } 
                        //if no match keep searching
                        else
                        {
                            matchFound = false;
                            continue;
                        }
                    }
                    //let user no if no match was found based on the description they entered
                    if (matchFound == false)
                    {
                        System.Console.WriteLine("Sorry, that description did not return a match. Please try again.");
                    }
                }                
                else
                {
                    Console.WriteLine("That ID did not return a match. Please try again.");
                }  

                //save any changes made in this method to the database
                todoContext.SaveChanges();
            }
        }

        //FUNCTION 3: MarkComplete take in todoContext and updates the status property to complete
        public static void MarkComplete(Context todoContext)
        {
            //begin loop to confirm and mark the ToDo complete
            bool loop = true;
            while (loop)
            {
                //prompt for displayID
                System.Console.WriteLine("Please enter the ID of the ToDo item you would like to mark complete:");

                //read the user's input 
                string input = Console.ReadLine(); 

                //Parse the input into an int if possible
                bool successfullyParsed = int.TryParse(input, out int parsedInput);

                var match = from t in todoContext.todos where t.displayID == parsedInput 
                select t;

                //if one unique match is returned, mark this item complete
                if (match.Count() == 1)
                {
                    foreach (Item item in match)
                    {
                        System.Console.WriteLine($"Thanks. Here's the current description of this ToDo: {item.description}");
                        System.Console.WriteLine("Enter any key to continue marking this ToDo complete OR enter 0 to keep this ToDo incomplete.");
                        string newStatus = Console.ReadLine();
                        if (newStatus != "0")
                        {
                            item.status = Status.complete;
                            item.displayID = 0;

                            System.Console.WriteLine("Your ToDo has been marked complete. Thank you!");
                            System.Console.WriteLine("");
      
                        }
                        else if (newStatus == "0")
                        {
                            System.Console.WriteLine("Great, it will stay incomplete. Thank you!");
                            System.Console.WriteLine("");
                        }
                    }
             
                    loop = false;  
                }

                //If the user has run multiple lists recently, there can be multiple equivalent display ids. 
                //In that case, guide the user to match Item by description
                else if (match.Count() > 1)
                {
                    System.Console.WriteLine("There are multiple ToDos with that ID. Sorry about that! Please enter the description of the item you'd like to mark complete.");
                    string descriptionMatch = Console.ReadLine().ToLower().Trim();
                    //tracking if a match is found. In case it's not, I want to display some feedback to the user.
                    bool matchFound = false;

                    foreach (Item item in match)
                    {
                        //if match complete the task and move on
                        if (descriptionMatch == item.description.ToLower().Trim())
                        {
                            item.status = Status.complete;
                            item.displayID = 0;

                            System.Console.WriteLine("Your ToDo has been marked complete. Thank you!");
                            System.Console.WriteLine("");
                            matchFound = true;
                            loop = false;
                            break;
                        }
                        //if no match keep searching
                        else
                        {
                            matchFound = false;
                            continue;
                        }
                    }
                    //let user no if no match was found based on the description they entered
                    if (matchFound == false)
                    {
                        System.Console.WriteLine("Sorry, that description did not return a match. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("That ID did not return a match. Please try again.");
                }    

                //save any changes made in this method to the database
                todoContext.SaveChanges();
            }
        }

        //FUNCTION 4: DELETE takes in the todoContext and removes a todo from the list
        public static void Delete(Context todoContext)
        {
            //begin loop to confirm and mark the ToDo complete
            bool loop = true;
            while (loop)
            {
                //prompt for displayID
                System.Console.WriteLine("Please enter the ID of the ToDo item you would like to delete:");

                //read the user's input 
                string input = Console.ReadLine(); 

                //Parse the input into an int if possible
                bool successfullyParsed = int.TryParse(input, out int parsedInput);

                //find a display ID match to the input
                var match = from t in todoContext.todos where t.displayID == parsedInput 
                select t;

                //if one match is found, delete it
                if (match.Count() == 1)
                {
                    foreach (Item item in match)
                    {
                        System.Console.WriteLine($"Thanks. Here's the current description of this ToDo: {item.description}");
                        System.Console.WriteLine("Enter any key to permanently delete this ToDo OR enter 0 to keep this ToDo in your database.");
                        string remove = Console.ReadLine();
                        if (remove != "0")
                        {
                            item.displayID = 0;
                            todoContext.todos.Remove(item);

                            System.Console.WriteLine("Your ToDo has been deleted. Thank you!");
                            System.Console.WriteLine("");
                        }
                        else if (remove == "0")
                        {
                            System.Console.WriteLine("Great, it will stay in the database. Thank you!");
                            System.Console.WriteLine("");
                        }
                    }

                    //exit loop
                    loop = false;                
                }

                //If the user has run multiple lists recently, there can be multiple equivalent display ids. 
                //In that case, guide the user to match Item by description
                else if (match.Count() > 1)
                {
                    System.Console.WriteLine("There are multiple ToDos with that ID. Sorry about that! Please enter the description of the item you'd like to mark complete.");
                    string descriptionMatch = Console.ReadLine().ToLower().Trim();
                    //tracking if a match is found. In case it's not, I want to display some feedback to the user.
                    bool matchFound = false;

                    foreach (Item item in match)
                    {
                        //if match complete the task and move on
                        if (descriptionMatch == item.description.ToLower().Trim())
                        {
                            item.displayID = 0;
                            todoContext.todos.Remove(item);

                            System.Console.WriteLine("Your ToDo has been deleted. Thank you!");
                            System.Console.WriteLine("");
                            matchFound = true;
                            loop = false;
                            break;
                        }
                        //if no match keep searching
                        else
                        {
                            matchFound = false;
                            continue;
                        }
                    }
                    //let user no if no match was found based on the description they entered
                    if (matchFound == false)
                    {
                        System.Console.WriteLine("Sorry, that description did not return a match. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("That ID did not return a match. Please try again.");
                }

                //save any changes made in this method to the database
                todoContext.SaveChanges();
            }
        }
        //FUNCTION 5: ListComplete takes in a todocontext and prints the todo items with a complete status to the screen
        public static void ListComplete(Context todoContext)
        {
            //find the complete todos in the list
            var results = from t in todoContext.todos where (t.status == Status.complete)
            select t;
            System.Console.WriteLine("My Completed ToDos:");
            WriteList(results);
            System.Console.WriteLine("");
            todoContext.SaveChanges();
        }
        //FUNCTION 6: takes in a todo context and returns all todos sorted by status (with incomplete todos first)
        public static void ListAll(Context todoContext)
        {
            //get all todos and order by status
            var results = from t in todoContext.todos orderby t.status ascending
            select t;
            System.Console.WriteLine("----------");
            System.Console.WriteLine("All ToDos:");
            System.Console.WriteLine("----------");
            
            //call function that prints the items based on the above results returned
            WriteList(results);
            System.Console.WriteLine("");

            //display ids need to be saved so that the user can select by them
            todoContext.SaveChanges();
        }
        //FUNCTION 7-9: take in a todoContext and an integer and list either Personal, Work, or school todos only
        public static void ListType(Context todoContext, int type)
        {
            //conditional to sort out which type we should list
            if (type == 7)
            {
                var results = from t in todoContext.todos where t.type == Type.personal
                select t;
                System.Console.WriteLine("------------------");
                System.Console.WriteLine("My Personal Todos:");
                System.Console.WriteLine("------------------");


                //call function that prints the items based on the above results returned
                WriteList(results);
            }
            else if (type == 8)
            {
                var results = from t in todoContext.todos where t.type == Type.work
                select t;
                System.Console.WriteLine("--------------");
                System.Console.WriteLine("My Work Todos:");
                System.Console.WriteLine("--------------");

                //call function that prints the items based on the above results returned
                WriteList(results);
            }
            else if (type == 9)
            {
                var results = from t in todoContext.todos where t.type == Type.school
                select t;
                System.Console.WriteLine("----------------");
                System.Console.WriteLine("My School Todos:");
                System.Console.WriteLine("----------------");

                //call function that prints the items based on the above results returned
                WriteList(results);
            }
            System.Console.WriteLine("");

            //display ids need to be saved so that the user can select by them
            todoContext.SaveChanges();
        }
        //Since many functions were ending with the same process for printing a list, I created a function to avoid copy/paste for the process of printing the list
        public static void WriteList(IQueryable<Item> results)
        {
            //Will display the following message if there are no todos in this results query
            if (results.Count() == 0)
            {
                System.Console.WriteLine("This ToDo list is empty.");
            }
            //if there are todos, list them
            else
            {
                //initialize counter
                int i = 0;

                //create and print heading
                string head1 = "ID";
                string head2 = "Description";
                string head3 = "Type";
                string head4 = "Deadline";
                string head5 = "Status";
                System.Console.WriteLine(head1.PadRight(3) + "| " + head2.PadRight(30) + "| " + head3.PadRight(25) + "| " + head4.PadRight(25) + "| " + head5.PadRight(15));
                System.Console.WriteLine("-".PadRight(102, '-'));

                foreach(Item item in results)
                {
                    i++;
                    item.displayID = i;
                    //use counter to set display id
                    if (item.deadline == DateTime.MinValue)
                    {
                        string column1 = item.displayID.ToString();
                        string column2 = item.description.ToString();
                        //display only the first 29 characters of a string if the todo is longer than 30 characters
                        if (column2.Length > 30)
                        {
                            column2 = column2.Substring(0,29);
                        }
                        string column3 = item.type.ToString();
                        string column4 = "no deadline";
                        string column5 = item.status.ToString();
                        System.Console.WriteLine(column1.PadRight(3) + "| " + column2.PadRight(30) + "| " + column3.PadRight(25) + "| " + column4.PadRight(25) + "| " + column5.PadRight(15));

                    }
                    else
                    {
                        string column1 = item.displayID.ToString();
                        string column2 = item.description.ToString();
                        //display only the first 29 characters of a string if the todo is longer than 30 characters
                        if (column2.Length > 30)
                        {
                            column2 = column2.Substring(0,29);
                        }
                        string column3 = item.type.ToString();
                        string column4 = item.deadline.ToString();
                        string column5 = item.status.ToString();
                        System.Console.WriteLine(column1.PadRight(3) + "| " + column2.PadRight(30) + "| " + column3.PadRight(25) + "| " + column4.PadRight(25) + "| " + column5.PadRight(15));

                    }
                }
            }
        }
    }
        
    //context class which creates the set of todos and calls on the Entity Framework to help link to the database
    public class Context:DbContext
    {
        public DbSet<Item> todos {get; set;}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //link to the database 
            optionsBuilder.UseSqlite("Filename=./todoapp.db");
        }
    }
}