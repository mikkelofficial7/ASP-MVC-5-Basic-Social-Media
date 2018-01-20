// Get the modal
var modal = document.getElementById('myModal');

// Get the button that opens the modal
var btn = document.getElementById("company-profile-term");
var btn2 = document.getElementById("company-profile-about");
var btn3 = document.getElementById("company-profile-privacy");
var val = document.getElementById("val");

var text1 = "<h2>Terms and Services</h2><br>Who May Use This<br />You may use the Services only if you agree to form a binding contract with HideID and are not a person barred from receiving services underthe laws of the applicable jurisdiction. In any case, you must be at least 15 years old to use the Services. If you are accepting these Terms and using the Services on behalf of a company, organization, government, or other legal entity, you represent and warrant that you are authorized to do so.<br /><br /> Content Of Services<br /> You are responsible for your use of the Services and for any Content you provide, including compliance with applicable laws, rules, and regulations.You should only provide Content that you are comfortable sharing with others. Any use or reliance on any Content or materials posted via the Services or obtained by you through the Services is at your own risk.We do not endorse, support, represent or guarantee the completeness, truthfulness, accuracy, or reliability of any Content or communications posted via the Services or endorse any opinions expressed via the Services.You understand that by using the Services, you may be exposed to Content that might be offensive, harmful, inaccurate or otherwise inappropriate, or in some cases, postings that have been mislabeled or are otherwise deceptive.All Content is the sole responsibility of the person who originated such Content.We may not monitor or control the Content posted via the Services and, we cannot take responsibility for such Content.<br /><br />Regards, <br /> HideID Management Support.";
var text2 = "<h2>About Us</h2><br/>HideID is an web app service that allowed people to make a tweet anonymously until about 150 characters maximum within their circle of friends and publicly. HideID have a text analysis which can analyze every person's tweet and make a conclusion in positive and negative percentage form. HideId is estabilished and inspired by predecor popular social media like Twitter, Secret, and Ask FM and combining them into one new social media with text analysis<br/><br/>Copyright © HideID Corp.<br/> Contact : <a id='contact' href='tel:+6283871979961'>+6283871979961</a><br/> Email : <a id='contact' href='mailto:mikkels86@gmail.com'>mikkels86@gmail.com</a>";
var text3 = "<h2>Privacy and Policy</h2><br>This Privacy Policy describes how and when we collect, use, and share information across websites related to this Policy (collectively the 'Services'), and from our partners and other third parties. For example, you submit information when using the Services on the web. When using our Services, you consent to the collection, transfer, storage, disclosure and use of your information as described in this Privacy Policy. This includes information you choose to grant that are considered sensitive under applicable law. If this policy specifies 'us' or 'us', this refers to the controller of your information under this policy. If you live in the United States, your information is controlled by HideID Indonesia Management. Nevertheless, you yourself control and are responsible for posting your Tweet and other content that you submit through the Service, as provided in the Terms of Service and the HideID Rules. Regardless of the country in which you live, you authorize us to transfer, store and use your information in the United States, Ireland and other countries where we operate. In some of these countries, privacy protection laws and regulations and data on when governments authorized to access data may differ from those in the country in which you live.<br/><br/>Based on <b>UU No.11 Tahun 2008 Pasal 26 Ayat 1 Republik Indonesia</b>";
// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

// When the user clicks on the button, open the modal 
btn.onclick = function () {
    modal.style.display = "block";
    val.innerHTML = text1;
}
btn2.onclick = function () {
    modal.style.display = "block";
    val.innerHTML = text2;
}
btn3.onclick = function () {
    modal.style.display = "block";
    val.innerHTML = text3;
}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}