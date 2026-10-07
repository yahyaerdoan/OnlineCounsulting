const maxWidth = 1200;
const maxHeight = 400;

function isBlank(data, index) {
    return data[index + 3] < 16 || (data[index] > 245 && data[index + 1] > 245 && data[index + 2] > 245);
}

export async function trimLogo(streamRef) {
    try {
        const bitmap = await createImageBitmap(new Blob([await streamRef.arrayBuffer()]));
        const source = new OffscreenCanvas(bitmap.width, bitmap.height);
        const sourceContext = source.getContext('2d');
        sourceContext.drawImage(bitmap, 0, 0);
        const { data, width, height } = sourceContext.getImageData(0, 0, bitmap.width, bitmap.height);

        let top = height, bottom = -1, left = width, right = -1;
        for (let y = 0; y < height; y++) {
            for (let x = 0; x < width; x++) {
                if (!isBlank(data, (y * width + x) * 4)) {
                    if (y < top) top = y;
                    if (y > bottom) bottom = y;
                    if (x < left) left = x;
                    if (x > right) right = x;
                }
            }
        }

        if (bottom < 0) {
            return null;
        }

        const pad = Math.round(Math.max(right - left, bottom - top) * 0.02);
        left = Math.max(0, left - pad);
        top = Math.max(0, top - pad);
        right = Math.min(width - 1, right + pad);
        bottom = Math.min(height - 1, bottom + pad);

        const cropWidth = right - left + 1;
        const cropHeight = bottom - top + 1;
        const scale = Math.min(1, maxWidth / cropWidth, maxHeight / cropHeight);
        const target = new OffscreenCanvas(Math.round(cropWidth * scale), Math.round(cropHeight * scale));
        target.getContext('2d').drawImage(source, left, top, cropWidth, cropHeight, 0, 0, target.width, target.height);

        const blob = await target.convertToBlob({ type: 'image/png' });
        return new Uint8Array(await blob.arrayBuffer());
    } catch {
        return null;
    }
}
