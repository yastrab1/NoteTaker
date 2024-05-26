using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Serialized;
using Json.Net;
var builder = WebApplication.CreateBuilder(args);
string cwd = Environment.CurrentDirectory;
NoteList noteList = new NoteList();
noteList = NoteListInit(cwd);

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseWebSockets();
app.MapPost("/new/", async context =>
{
    string body = await new StreamReader(context.Request.Body).ReadToEndAsync();
    Note note = JsonNet.Deserialize<Note>(body);
    note.guid = Guid.NewGuid();
    noteList.notes.Add(note);

    using (StreamWriter outputFile = new StreamWriter(Path.Combine(cwd, "Notes.txt")))
    {
        await outputFile.WriteAsync(JsonNet.Serialize(noteList));
    }
});
app.MapGet("/get/{guid}", async context =>
{
    string guid = context.Request.Path.ToUriComponent().Substring(5);
    foreach (var note in noteList.notes)
    {
        Console.WriteLine(note.guid.ToString() + " " + guid);
        if (note.guid.ToString() == guid)
        {
            
            await context.Response.WriteAsync(JsonNet.Serialize(note));
            return;
        }
    }
    context.Response.StatusCode = 400;
    await context.Response.WriteAsync("Not found");
});
app.MapGet("/connect/{boardId}", async context =>
{
    
});
app.Run();

NoteList NoteListInit(string s)
{
    NoteList noteList1;
    using (StreamReader reader = new StreamReader(Path.Combine(s, "Notes.txt")))
    {
        noteList1 = JsonNet.Deserialize<NoteList>(reader.ReadToEnd());
    }

    return noteList1;
}
