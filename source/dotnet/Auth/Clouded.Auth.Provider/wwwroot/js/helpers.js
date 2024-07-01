window.previewImage = (imgBytes, elemId) => {
    const url = URL.createObjectURL(new Blob([imgBytes]));
    const imgElem = document.getElementById(elemId);
    imgElem.addEventListener('load', () => URL.revokeObjectURL(url), {once: true});
    imgElem.src = url;
}

window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}