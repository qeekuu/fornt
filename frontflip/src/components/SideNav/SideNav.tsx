import { Link } from "react-router-dom";
import styles from "./SideNav.module.css";

function SideNav () {

  return (
	<div className={styles.leftNav}>
		<div className={styles.leftNavLogo}>
			<p>FrontFlip</p>
		</div>

		<nav className={styles.leftNavMenu}>
			<input
				type="text"
				placeholder="Szukaj..."
				className={styles.searchInput}
			/>
			
			<Link to="/home" className={styles.leftNavItem}>
				<span className={styles.sideNavLabel}>Home</span>
			</Link>
			<Link to="/gallery" className={styles.leftNavItem}>
				<span className={styles.sideNavLabel}>Galeria zdjęć</span>
			</Link>
			<Link to="/addPhoto" className={styles.leftNavItem}>
				<span className={styles.sideNavLabel}>Dodaj zdjęcie</span>
			</Link>

		</nav>
		</div>
	);
}

export default SideNav;
