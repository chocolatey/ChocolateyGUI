namespace ChocolateyGui.Subprocess.Models
{
    public class ChocolateyFeature
    {
        public string Name { get; set; }
        
        public bool Enabled { get; set; }
        
        public bool SetExplicitly { get; set; }
        
        public string Description { get; set; }
    }
}
