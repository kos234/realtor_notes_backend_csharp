using realtor_notes_backend.base_note.model;

namespace realtor_notes_backend.client.model;

public class ClientNote : Note
{
    public string FullName {get; set;}
    public List<string> Phones {get; set;} = new List<string>();
}