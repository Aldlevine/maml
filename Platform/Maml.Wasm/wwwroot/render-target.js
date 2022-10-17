var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { wasm } from "./wasm.js";
var FontStyle;
(function (FontStyle) {
    FontStyle[FontStyle["Normal"] = 0] = "Normal";
    FontStyle[FontStyle["Oblique"] = 1] = "Oblique";
    FontStyle[FontStyle["Italic"] = 2] = "Italic";
})(FontStyle || (FontStyle = {}));
;
var FlowDirection;
(function (FlowDirection) {
    FlowDirection[FlowDirection["LeftToRight"] = 0] = "LeftToRight";
    FlowDirection[FlowDirection["RightToLeft"] = 1] = "RightToLeft";
})(FlowDirection || (FlowDirection = {}));
;
var WrappingMode;
(function (WrappingMode) {
    WrappingMode[WrappingMode["Normal"] = 0] = "Normal";
    WrappingMode[WrappingMode["None"] = 1] = "None";
    WrappingMode[WrappingMode["Character"] = 2] = "Character";
    WrappingMode[WrappingMode["Word"] = 3] = "Word";
})(WrappingMode || (WrappingMode = {}));
;
var WasmDrawCommand;
(function (WasmDrawCommand) {
    WasmDrawCommand[WasmDrawCommand["Clear"] = 0] = "Clear";
    WasmDrawCommand[WasmDrawCommand["SetTransform"] = 1] = "SetTransform";
    WasmDrawCommand[WasmDrawCommand["FillRect"] = 2] = "FillRect";
    WasmDrawCommand[WasmDrawCommand["StrokeRect"] = 3] = "StrokeRect";
    WasmDrawCommand[WasmDrawCommand["FillGeometry"] = 4] = "FillGeometry";
    WasmDrawCommand[WasmDrawCommand["StrokeGeometry"] = 5] = "StrokeGeometry";
    WasmDrawCommand[WasmDrawCommand["FillText"] = 6] = "FillText";
    WasmDrawCommand[WasmDrawCommand["PushClip"] = 7] = "PushClip";
    WasmDrawCommand[WasmDrawCommand["PopClip"] = 8] = "PopClip";
})(WasmDrawCommand || (WasmDrawCommand = {}));
;
class RenderTarget {
    constructor() {
        this.canvases = {};
        this.contexts = {};
        this.textMeasurer = document.getElementById("text-measurer");
        this.geometries = {};
        this.currentGeometryId = 0;
        this.brushes = {};
        this.currentBrushId = 0;
        this.texts = {};
        this.currentTextId = 0;
    }
    init() {
        return __awaiter(this, void 0, void 0, function* () {
            wasm.setModuleImports("render-target.js", this);
            this.interop = yield wasm.getAssemblyExports("Maml.Wasm", "Maml.RenderTarget");
            this.canvases[0] = document.getElementById("canvas");
            this.contexts[0] = this.canvases[0].getContext("2d", {
                alpha: false,
            });
        });
    }
    // Drawing
    processDrawCommands(canvasId, commandBuffer) {
        let cmdIdx = 0;
        const ctx = this.contexts[canvasId];
        while (cmdIdx < commandBuffer.length) {
            let cmd = commandBuffer[cmdIdx++];
            switch (cmd) {
                case WasmDrawCommand.Clear:
                    {
                        const r = commandBuffer[cmdIdx++];
                        const g = commandBuffer[cmdIdx++];
                        const b = commandBuffer[cmdIdx++];
                        const a = commandBuffer[cmdIdx++];
                        this.clear(ctx, r, g, b, a);
                    }
                    break;
                case WasmDrawCommand.SetTransform:
                    {
                        const matrixArray = new Float64Array(commandBuffer.slice(cmdIdx, cmdIdx + 6));
                        cmdIdx += 6;
                        this.setTransform(ctx, matrixArray);
                    }
                    break;
                case WasmDrawCommand.FillGeometry:
                    {
                        const geometryId = commandBuffer[cmdIdx++];
                        const brushId = commandBuffer[cmdIdx++];
                        this.fillGeometry(ctx, geometryId, brushId);
                    }
                    break;
                case WasmDrawCommand.StrokeGeometry:
                    {
                        const geometryId = commandBuffer[cmdIdx++];
                        const brushId = commandBuffer[cmdIdx++];
                        const thickness = commandBuffer[cmdIdx++];
                        this.strokeGeometry(ctx, geometryId, brushId, thickness);
                    }
                    break;
                case WasmDrawCommand.FillRect:
                    {
                        const x = commandBuffer[cmdIdx++];
                        const y = commandBuffer[cmdIdx++];
                        const w = commandBuffer[cmdIdx++];
                        const h = commandBuffer[cmdIdx++];
                        const brushId = commandBuffer[cmdIdx++];
                        this.fillRect(ctx, x, y, w, h, brushId);
                    }
                    break;
                case WasmDrawCommand.StrokeRect:
                    {
                        const x = commandBuffer[cmdIdx++];
                        const y = commandBuffer[cmdIdx++];
                        const w = commandBuffer[cmdIdx++];
                        const h = commandBuffer[cmdIdx++];
                        const brushId = commandBuffer[cmdIdx++];
                        const thickness = commandBuffer[cmdIdx++];
                        this.strokeRect(ctx, x, y, w, h, brushId, thickness);
                    }
                    break;
                case WasmDrawCommand.FillText:
                    {
                        const textId = commandBuffer[cmdIdx++];
                        const brushId = commandBuffer[cmdIdx++];
                        this.fillText(ctx, textId, brushId);
                    }
                    break;
                case WasmDrawCommand.PushClip:
                    {
                        const x = commandBuffer[cmdIdx++];
                        const y = commandBuffer[cmdIdx++];
                        const w = commandBuffer[cmdIdx++];
                        const h = commandBuffer[cmdIdx++];
                        this.pushClip(ctx, x, y, w, h);
                    }
                    break;
                case WasmDrawCommand.PopClip:
                    {
                        this.popClip(ctx);
                    }
                    break;
            }
        }
    }
    clear(ctx, r, g, b, a) {
        ctx.resetTransform();
        const color = `rgba(${r * 255},${g * 255},${b * 255},${a})`;
        ctx.fillStyle = color;
        ctx.fillRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        document.body.style.background = color;
    }
    setTransform(ctx, matrixArray) {
        ctx.resetTransform();
        ctx.scale(devicePixelRatio, devicePixelRatio);
        ctx.transform(matrixArray[0], matrixArray[1], matrixArray[2], matrixArray[3], matrixArray[4], matrixArray[5]);
    }
    fillGeometry(ctx, geometryId, brushId) {
        ctx.fillStyle = this.brushes[brushId];
        ctx.fill(this.geometries[geometryId]);
    }
    strokeGeometry(ctx, geometryId, brushId, thickness) {
        ctx.strokeStyle = this.brushes[brushId];
        ctx.lineWidth = thickness;
        ctx.stroke(this.geometries[geometryId]);
    }
    fillRect(ctx, x, y, w, h, brushId) {
        ctx.fillStyle = this.brushes[brushId];
        ctx.fillRect(x, y, w, h);
    }
    strokeRect(ctx, x, y, width, height, brushId, thickness) {
        ctx.strokeStyle = this.brushes[brushId];
        ctx.lineWidth = thickness;
        ctx.strokeRect(x, y, width, height);
    }
    fillText(ctx, textId, brushId) {
        ctx.fillStyle = this.brushes[brushId];
        const text = this.texts[textId];
        ctx.font = `${text.fontWeight} ${text.fontSize}px "${text.fontName}"`;
        ctx.textBaseline = "top";
        const baseline = (text.lineHeight - text.fontSize) / 2;
        for (let i = 0, ii = text.lines.length; i < ii; i++) {
            ctx.fillText(text.lines[i], 0, baseline + (i * text.lineHeight));
        }
    }
    pushClip(ctx, x, y, width, height) {
        ctx.save();
        ctx.beginPath();
        ctx.rect(x, y, width, height);
        ctx.clip("nonzero");
    }
    popClip(ctx) {
        ctx.restore();
    }
    // Resources
    releaseGeometry(id, geometryId) {
        delete this.geometries[geometryId];
    }
    makeRectGeometry(id, x, y, w, h) {
        const path = new Path2D();
        path.rect(x, y, w, h);
        this.geometries[this.currentGeometryId] = path;
        return this.currentGeometryId++;
    }
    makeEllipseGeometry(id, x, y, radiusX, radiusY) {
        const path = new Path2D();
        path.ellipse(x, y, radiusX, radiusY, 0, 0, Math.PI * 2);
        this.geometries[this.currentGeometryId] = path;
        return this.currentGeometryId++;
    }
    makeLineGeometry(id, startX, startY, endX, endY) {
        const path = new Path2D();
        path.moveTo(startX, startY);
        path.lineTo(endX, endY);
        this.geometries[this.currentGeometryId] = path;
        return this.currentGeometryId++;
    }
    releaseBrush(id, brushId) {
        delete this.brushes[brushId];
    }
    makeColorBrush(id, color) {
        this.brushes[this.currentBrushId] = color;
        return this.currentBrushId++;
    }
    releaseText(id, textId) {
        delete this.texts[textId];
    }
    makeText(id, text, wrappingMode, lineHeight, fontName, fontSize, fontStyle, fontWeight, maxSizeX, maxSizeY) {
        this.textMeasurer.style.font = `${fontWeight} ${fontSize}px "${fontName}"`;
        this.textMeasurer.innerText = " ";
        const spaceWidth = this.textMeasurer.getClientRects()[0].width;
        switch (wrappingMode) {
            case WrappingMode.None:
                this.textMeasurer.className = "none";
                break;
            case WrappingMode.Normal:
                this.textMeasurer.className = "normal";
                break;
            case WrappingMode.Word:
                this.textMeasurer.className = "word";
                break;
            case WrappingMode.Character:
                this.textMeasurer.className = "character";
                // insert zero-width space between each character
                text = [...text].join("\u200b");
                break;
        }
        this.textMeasurer.style.width = `${maxSizeX}px`;
        this.textMeasurer.innerText = text;
        for (let textNode of this.textMeasurer.childNodes) {
            if (!(textNode instanceof Text)) {
                continue;
            }
            while (textNode.length > 1) {
                textNode.splitText(textNode.length - 1);
            }
        }
        const range = document.createRange();
        range.selectNodeContents(this.textMeasurer);
        const lines = [];
        let curLine = "";
        let lastTop = 0;
        let lastRight = 0;
        let width = 0;
        for (let t of this.textMeasurer.childNodes) {
            range.selectNode(t);
            const rect = range.getClientRects()[0];
            width = Math.max(rect.right, width);
            if (rect.top > lastTop) {
                // new line
                if (curLine != "") {
                    lines.push(curLine);
                }
                curLine = t.textContent;
            }
            else {
                if (rect.left > lastRight) {
                    if (spaceWidth > 0) {
                        const numSpaces = Math.floor((rect.left - lastRight) / spaceWidth);
                        const spaces = Array(numSpaces).fill(" ").join("");
                        curLine += spaces;
                    }
                }
                curLine += t.textContent;
            }
            lastTop = rect.top;
            lastRight = rect.right;
        }
        if (curLine != "") {
            lines.push(curLine);
        }
        const textObj = {
            fontName,
            fontSize,
            fontStyle,
            fontWeight,
            lineHeight,
            lines,
        };
        this.texts[this.currentTextId] = textObj;
        const height = lines.length * lineHeight;
        return new Float64Array([this.currentTextId++, lines.length, width, height]);
    }
}
export const renderTarget = new RenderTarget();
//# sourceMappingURL=render-target.js.map