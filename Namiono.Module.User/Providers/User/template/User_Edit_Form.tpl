<section id="user">
	<h2>Benutzerkonto bearbeiten</h2>
	<hr>

	<form action="/provider/user/edit/" id="User_Edit_Form" method="post" enctype="application/x-www-form-urlencoded">
		<input class="input_text" type="email" tabindex="1" name="useremail" placeholder="E-Mail"><br>
		<input class="input_text" type="text" tabindex="2" name="username" placeholder="Benutzername"><br><br>
		
		<input class="input_text" type="password" tabindex="3" name="userpass" placeholder="Passwort"><br>
		<input class="input_text" type="password" tabindex="4" name="userpassb" placeholder="Passwort (Wiederholung)"><br>

		<input type="submit" name="submit" value="Absenden" class="button">
	</form>

</section>