(function() {
    var divElements = [].slice.call(document.querySelectorAll("#load"));
    var loadMore = document.querySelector('#loadMore');
    var divNumber = 5;
	  
    loadMore.addEventListener('click', function(e) {
        e.preventDefault();
        for (var i = 0; i < divNumber; i++) {
		window.scrollTo(0,document.body.scrollHeight);
            if (i < divElements.length) {
                divElements[i].style.display = 'block';
            }

            if (i >= divElements.length) {
                $("#loadMore").css('display', 'none');
                loadMore.innerHTML = "Load Completed";
				return;
            }

        }
        divElements.splice(0, divNumber);

    });
})();
  loadMore.click();