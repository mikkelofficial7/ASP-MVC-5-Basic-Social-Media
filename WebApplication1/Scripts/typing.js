function wordCount(val) {
    var wom = val.match(/\S+/g);
    return {
        charactersNoSpaces: val.replace(/\s+/g, '').length,
        characters: val.length,
        words: wom ? wom.length : 0,
        lines: val.split(/\r*\n/).length
    };
}
var textarea = document.getElementById("tweet-text");
var result = document.getElementById("val");

textarea.addEventListener("input", function () {
    var v = wordCount(this.value);
    result.innerHTML = (" "+v.characters+" of 150");
}, false);