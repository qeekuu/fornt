import type {FormEvent, JSX} from "react";
import AnimatedPage from "../../components/AnimatedPage.tsx";
import styles from "./LoginPage.module.css";
import {Link, useNavigate} from "react-router-dom";
import {xssPrevent} from "../../utils/func.ts";
import {apiUrls} from "../../utils/constants.ts";

function LoginPage(): JSX.Element {
	const navigate = useNavigate();

  function onSubmit(event: FormEvent<HTMLFormElement>): void {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);
    let email = formData.get("email") as string;
    let password = formData.get("password") as string;

    email = xssPrevent(email);
    password = xssPrevent(password);

    fetch(apiUrls.LOGIN_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email,
        password,
      }),
    })
      .then((res) => {
        if (res.ok) {
          alert("Zalogowano pomyślnie!");
		  navigate("/home");
        }
        console.log(res);
      })
      .catch((error) => {
        alert(`Błąd sieci: ${error.message}`);
      });
  }

  return <AnimatedPage className={styles.loginPage}>
    <h2>Witaj spowrotem!!!</h2>
    <form onSubmit={onSubmit} className={styles.form}>
      <input type="email" name="email" required placeholder={"Email"}/>
      <input type="password" name="password" required placeholder={"Hasło"}/>
      <div className={styles.buttonRow}>
        <button type={"submit"} className={"button"}>Zaloguj się</button>
        <Link to={"/register"} className={"button"}>Zarejestruj się</Link>
      </div>
    </form>
  </AnimatedPage>;
}

export default LoginPage;

