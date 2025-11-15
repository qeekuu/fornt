import {type FormEvent, type JSX} from "react";
import AnimatedPage from "../../components/AnimatedPage.tsx";
import styles from "./LoginPage.module.css";
import {Link} from "react-router-dom";
import {xssPrevent} from "../../utils/func.ts"
import {apiUrls} from "../../utils/constants.ts";

function RegisterPage(): JSX.Element {
  function onSubmit(event: FormEvent<HTMLFormElement>): void {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    let username = formData.get("username") as string;
    let firstname = formData.get("firstname") as string;
    let lastname = formData.get("lastname") as string;
    let email = formData.get("email") as string;
    let password = formData.get("password") as string;
    const confirmPassword = formData.get("confirmPassword") as string;
    const termsAccepted = formData.get("terms") === "on";
    const dataProcessingAccepted = formData.get("dataProcessing") === "on";

    username = xssPrevent(username);
    firstname = xssPrevent(firstname);
    lastname = xssPrevent(lastname);
    email = xssPrevent(email);
    password = xssPrevent(password);

    if (password !== confirmPassword) {
      alert("Hasła nie są zgodne!");
      return;
    }

    if (!termsAccepted || !dataProcessingAccepted) {
      alert("Musisz zaakceptować regulamin i przetwarzanie danych osobowych!");
      return;
    }

    fetch(apiUrls.REGISTER_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        login: username,
        name: firstname,
        surname: lastname,
        email,
        password,
      }),
    })
      .then(async (response) => {
        if (response.ok) {
          alert("Rejestracja zakończona sukcesem! Możesz się teraz zalogować.");
        } else {
          const data = await response.json();
          alert(`Błąd rejestracji: ${data.message || response.statusText}`);
        }
      })
      .catch((error) => {
        alert(`Błąd sieci: ${error.message}`);
      });
  }

  return <AnimatedPage>
    <h2>Dołącz do FrontFlip!</h2>
    <form onSubmit={onSubmit} className={styles.form}>
      <input type="text" name="username" required placeholder={"Nazwa użytkownika"}/>
      <input type="text" name="firstname" required placeholder={"Imię"}/>
      <input type="text" name="lastname" required placeholder={"Nazwisko"}/>
      <input type="email" name="email" required placeholder={"Email"}/>
      <input type="password" name="password" required placeholder={"Hasło"}/>
      <input type="password" name="confirmPassword" required placeholder={"Potwierdź hasło"}/>
      <br/>
      <label>
        <input type="checkbox" name="terms"/>
        <div>
          <span></span>
        </div>
        <span>Akceptuję <a href={"/regulamin.txt"} target={"_blank"}>regulamin</a></span>
      </label>
      <label>
        <input type="checkbox" name="dataProcessing"/>
        <div>
          <span></span>
        </div>
        <span>Zgadzam się na przetwarzanie moich danych osobowych</span>
      </label>
      <div className={styles.buttonRow}>
        <button type={"submit"} className={"button"}>Zarejestruj się</button>
        <Link to={"/login"} className={"button"}>Zaloguj się</Link>
      </div>
    </form>
  </AnimatedPage>;
}

export default RegisterPage;

