﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Graphics;

public record DrawLayer { }
public record Stroke(Brush Brush, int Thickness) : DrawLayer { }
public record Fill(Brush Brush) : DrawLayer { }
