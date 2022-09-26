namespace Maml.Graphics;

public record DrawLayer(Brush Brush) { }
public record Stroke(Brush Brush, int Thickness) : DrawLayer(Brush) { }
public record Fill(Brush Brush) : DrawLayer(Brush) { }
