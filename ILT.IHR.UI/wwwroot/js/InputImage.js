function getFileDetails(fileName) {
    //returns base64 string here
    var file;
    if (!fileName.includes("pdf")) {
        file = document.querySelector("#output");
    } else {
        file = document.querySelector("#inputFile");
    }
    var matches = file.src.match(/^data:([A-Za-z-+\/]+);base64,(.+)$/);
    var ImageBase64 = matches[2];
    return ImageBase64;
}


function process() {
    const file = document.querySelector("#upload").files[0];

    if (!file) return;

    const reader = new FileReader();
    reader.readAsDataURL(file);
    if (!file.name.includes("pdf")) {
        reader.onload = function (event) {
            const imgElement = document.createElement("img");
            imgElement.src = event.target.result;
            document.querySelector("#input").src = event.target.result;
            imgElement.onload = function (e) {
                const canvas = document.createElement("canvas");
                const MAX_WIDTH = 400;
                const scaleSize = MAX_WIDTH / e.target.width;
                canvas.width = MAX_WIDTH;
                canvas.height = e.target.height * scaleSize;
                const ctx = canvas.getContext("2d");
                ctx.drawImage(e.target, 0, 0, canvas.width, canvas.height);
                const srcEncoded = ctx.canvas.toDataURL(e.target, "image/jpeg");
                // you can send srcEncoded to the server
                document.querySelector("#output").src = srcEncoded;
            };
        };
    } else if (file.name.includes("pdf")) {
        reader.onload = function (event) {
            document.querySelector("#inputFile").src = event.target.result;
        }
    }
    
    return file.name;
}

function FileSaveAs(base64) {
    var link = document.createElement('a');
    link.download = "LeaveSummary.xlsx";
    link.href = "data:application/octet-stream;base64," + base64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}