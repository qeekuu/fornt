import type { JSX } from "react";
import { Link } from "react-router-dom";
import styles from "./HomePage.module.css";

function HomePage(): JSX.Element {
  return (
    <div className={styles.homePage}>
      <div className={styles.homeInner}>
        <header className={styles.homeHeader}>
          <h1>FrontFlip</h1>
          <p>Przeglądaj, dodawaj i udostępniaj swoje zdjęcia.</p>
        </header>

        <section className={styles.cardsRow}>
          <div className={styles.previewCard}>
            <div className={styles.previewImage}></div>
            <div className={styles.previewText}>
              <h2>Ostatnio dodane</h2>
              <p>Ostatnie zdjęcie.</p>
            </div>
          </div>

          <Link to="/addPhoto" className={styles.addCard}>
            <div className={styles.addIcon}>+</div>
            <p>Dodaj zdjęcie</p>
          </Link>
        </section>
      </div>
    </div>
  );
}

export default HomePage;

