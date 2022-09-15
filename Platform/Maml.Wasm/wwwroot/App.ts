/// <reference path="" />
/// <reference path="Input.ts" />
/// <reference path="Renderer.ts" />

const input: Input = new Input();
(window as any).Input = input;

const renderer: Renderer = new Renderer();
(window as any).Renderer = renderer;

input.Run();
