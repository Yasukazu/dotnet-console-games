using LiteDB;

// Open database (or create if doesn't exist)
using(var db = new LiteDatabase(@"MyData.ltdb"))
{
    // Get a collection (or create, if doesn't exist)
    var col = db.GetCollection<Customer>("customers");

    // Create your new customer instance
    var customer = new Customer
    { 
        Name = "John Doe", 
        Phones = new string[] { "8000-0000", "9000-0000" }, 
        IsActive = true
    };
	
    // Insert new customer document (Id will be auto-incremented)
    col.Insert(customer);
	
    // Update a document inside a collection
    customer.Name = "Joana Doe";
	
    col.Update(customer);
	
    // Index document using document Name property
    col.EnsureIndex(x => x.Name);
	
    // Use LINQ to query documents
     var results = col.Find(x => 
        x.Name.StartsWith("Jo"));

    // Let's create an index in phone numbers (using expression). It's a multikey index
    //col.EnsureIndex(x => x.Phones, "$.Phones[*]"); 

    // and now we can query phones
    var r = col.FindOne(x => x.Phones.Contains("8888-5555"));
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string[] Phones { get; set; } = {"",};
    public bool IsActive { get; set; }
}
