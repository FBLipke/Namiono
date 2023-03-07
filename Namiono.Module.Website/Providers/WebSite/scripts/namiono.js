var dragWindow = null;
var dragx = 0;
var dragy = 0;
var posx = 0;
var posy = 0;

function DispatchResponse(provider, action, windowId, data) {
	if (data.length <= 2)
		return "Die zurückgegebe Antwort war leer!";

	var jsonResponse = JSON.parse(data);
    var output = "";

    if (action == "Get") {
        for (var i = 0; i < jsonResponse.length; i++) {
            output += "<div class=\"td\">" + jsonResponse[i].Name + "</div>" +
                "<div class=\"td\">" + jsonResponse[i].Description + "</div>";
        }
    }

	return output;
}

function SetWindowForground(id) {
	var winlist = document.getElementsByClassName("box");
	var lastindex = 0;
	if (winlist == null) {
		return false;
	}

	var i;
	for (i = 0; i < winlist.length; i++) {
		if (lastindex !== 0)
			winlist[i].style.zIndex = (lastindex - 1);
		else
			lastindex = winlist[i].style.zIndex;
	}

	for (i = 0; i < winlist.length; i++) {
		if (winlist[i].id === id) {
			winlist[i].style.zIndex = 101;
			return true;
		}
	}

	return false;
}

function RemoveWindow(id) {
	var window = GetWindowById(id);
	if (window != null) {
		window.style.display = "none";
		window.remove();
		return true;
	}

	return false;
}

function Update_WindowStatusText(id, text) {
	var window = GetWindowById(id);
	if (window == null)
		return false;

	window.lastChild.innerText = text;
	return true;
}

function GetWindowById(id) {
	return document.getElementById(id);
}

function GetWindowSurface(windowId) {
	var win = GetWindowById(windowId);

	if (win == null)
		return null;

	var surface = win.getElementsByClassName("box_content");
	if (surface == null) {
		console.error("Namiono: Failed to get Window Surface");
		return null;
	}

	return surface;
}

function UpdateWindowSurface(windowId, content) {
	var surface = GetWindowSurface(windowId);
	if (surface == null) {
		console.error("Namiono: Failed to get Window Surface");
		return false;
	}

	surface.innerHTML = content;
	return true;
}

function Create_ContentBox(title, action, content, windowId, width, height) {

	// Haben wir bereits ein Fenster mit dieser Id?

	var window = GetWindowById(windowId);
	var url = "";
	
	if (window == null) {
		var output = "<div draggable=\"false\" class=\"box\" id=\"" + windowId + "\">\n";
		output += "<div draggable=\"true\" class=\"box_header\" onmousedown=\"dragstart('" + windowId + "');\">" + title + "</div>\n";
		output += "<div draggable=\"false\" class=\"box_close_icon\" onmousedown=\"RemoveWindow('" + windowId + "');\"></div>\n";
		output += "<div draggable=\"false\" class=\"box_content\" onmousedown=\"SetWindowForground('" + windowId + "');\">\n";
		output += content;
		output += "</div>\n";
		output += "<div draggable=\"false\" class=\"box_footer\" onmousedown=\"SetWindowForground('" + windowId + "');\">Bereit...</div>\n";
		output += "</div>\n";

		var main = document.getElementsByTagName("MAIN")[0];
		main.innerHTML = main.innerHTML + output;
		main.addEventListener("mousemove", drag);
		main.addEventListener("mouseup", dragstop);

		window = GetWindowById(windowId);
	}

	SetWindowBounds(window.id, width, height);
	SetWindowForground(window.id);

	return true;
}

function SetWindowPosition(windowId, x, y) {
	var win = GetWindowById(windowId);

	if (win == null)
		return false;

	win.style.left = x + "px";
	win.style.top = y + "px";

	return true;
}

function SetWindowBounds(windowId, w, h) {
	var win = GetWindowById(windowId);
	if (win == null)
		return false;

	win.clientWidth = w;
	win.clientHeight = h;
	
	for (var i = 0; i < win.childCount; i++) {
		if (i === 0) {
			win.childNodes[i].clientWidth = win.clientWidth - 18;
			win.childNodes[i].clientHeight = win.clientheight;
		} else {
			win.childNodes[i].clientWidth = win.clientWidth;
			win.childNodes[i].clientHeight = win.clientheight;
		}
	}
	
	return true;
}

function dragstart(windowId) {
	var window = GetWindowById(windowId);
	if (window == null) {
		return false;
	}

	SetWindowForground(window);

	dragWindow = window;
	dragWindow.style.position = "absolute";
	dragWindow.style.opacity = "0.8";

	dragx = posx - dragWindow.offsetLeft;
	dragy = posy - dragWindow.offsetTop;

	
	return true;
}

function dragstop() {
	if (dragWindow != null) {
		dragWindow.style.opacity = "1.0";
		SetWindowForground(dragWindow.id);
		dragWindow = null;

		return true;
	}

	return false;
}

function drag(ereignis) {
	posx = document.all ? window.event.clientX : ereignis.pageX;
	posy = document.all ? window.event.clientY : ereignis.pageY;

	if (dragWindow != null) {
		SetWindowPosition(dragWindow.id, (posx - dragx), (posy - dragy));
		return true;
	}

	return false;
}

function RequestJSON(provider, realm, windowId, action, parameters = "", method = "") {
	var accepts = "Application/json";
	if (method === "") {
		method = "GET";
	}
	else {
		method = "POST";
	}

	var request = window.$.ajax({
		url: "/providers/" + provider + "/" + action + "/",
		method: method,
		cache: false,
		async: true,
		dataType: "json",
		data: JSON.stringify(parameters),
		accepts: accepts,
		beforeSend: function (req) {
            req.setRequestHeader("Action", btoa(action));
			req.setRequestHeader("Window", btoa(windowId));
			req.setRequestHeader("Realm", btoa(realm));
        },
        success: function (res) {
            Update_WindowStatusText(windowId, "Verbunden (" + res.responseText + "); Warte auf antwort...");
		},
        error: function (response, status, text) {
            var action = response.getResponseHeader("Action");
            Create_ContentBox(document.title, action, text, windowId, 400, 200);
	        Update_WindowStatusText(windowId, "Fehler!");
		},
		complete: function (response) {
			var clen = response.getResponseHeader("Content-Length");
			if (clen !== "0") {
                var prov = response.getResponseHeader("Provider");
                var _action = response.getResponseHeader("Action");
				var _realm = response.getResponseHeader("Realm");
				var output = DispatchResponse(prov, _realm, _action, windowId, response.responseText);
				if (_realm == "admin") {
					Create_ContentBox(document.title, _action, output, windowId, "600", "400");
					Update_WindowStatusText(windowId, "Fertig!");
				} else {
					document.getElementsByTagName("MAIN")[0].innerHTML = output;
				}
			} else {
				if (_realm == "admin") {
					Update_WindowStatusText(windowId, "Leere Antwort!");
				}
			}
		}
	});
}

function RequestHTML(provider, action, parameters = null, method = "") {
	var accepts = "text/html";
	if (method === "") {
		method = "GET";
	}

	var request = window.$.ajax({
		url: "/provider/" + provider + "/" + action + "/",
		method: method,
		cache: false,
		async: true,
		dataType: "json",
		data: JSON.stringify(parameters),
		accepts: accepts,
		beforeSend: function (req) {
			req.setRequestHeader("Action", btoa(action));
			req.setRequestHeader("Provider", btoa(provider));
		},
		success: function (res) {
		},
		error: function (response, status, text) {
		},
		complete: function (response) {
			document.getElementsByTagName("MAIN")[0].innerHTML = response.responseText;
		}
	});

	return false;
}

function Split(data, delimeter) {
    return data.split(delimeter);
}

function Get_ExtraData_Replace(data, text)
{
    var dataParts = Split(data);

    for (var ic = 0; ic < dataParts.length; ic++)
    {
        if (data[ic] === "replace")
        {
            var paterns = data[(ic + 1)].split("=>");
            text = window.Replace(text, paterns[0], paterns[1]);
        }
    }

    return text;
}