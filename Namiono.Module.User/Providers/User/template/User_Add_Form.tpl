<section id="user">
	<h2>Benutzerkonto erstellen</h2>
	<hr>

<p class="note_text">
		Hier kannst Du ein Benutzerkonto erstellen. Wir müssen dafür einige Angaben dafür erheben.<br />
		Welche Angaben das sind, findest Du im unten stehenden Formular.

		Nach dem Absenden der Angaben wird deine E-Mail Adresse auf <a href="https://www.stopforumspam.com/">StopForumSpam</a><br />
		auf (eventuelle vergangene) verdächtige aktivitäten überprüft.

		Je nach Ergebnis der Überprüfung, kann die Erstellung des Benutzerkontos verwehrt oder genemigt werden. Sollte die Überprüfung fehlschlagen,
		so kannst Du dich an einem Administrator wenden.<br />
</p>

	<h2>Mit dem Absenden der eingegebenen Angaben stimmts Du zu:</h2>

	<ul>
		<li>Dass deine E-Mail Adresse auf (eventuelle vergangene) verdächtige Aktivitäten auf <a href="https://www.stopforumspam.com/">StopForumSpam</a> überprüft wird.</li>
		<li>Du hast den obigen Text vollständig gelesen und verstanden.</li>
	</ul>

	<form action="/provider/user/add/" id="User_Add_Form" method="post" enctype="application/x-www-form-urlencoded">
		<input class="input_text" type="email" tabindex="1" name="useremail" placeholder="E-Mail"><br>
		<input class="input_text" type="text" tabindex="2" name="username" placeholder="Benutzername"><br><br>
		
		<input class="input_text" type="password" tabindex="3" name="userpass" placeholder="Passwort"><br>
		<input class="input_text" type="password" tabindex="4" name="userpassb" placeholder="Passwort (Wiederholung)"><br>

		<input type="submit" name="submit" value="Absenden" class="button">
	</form>

</section>