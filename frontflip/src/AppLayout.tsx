import type { JSX } from "react";
import AnimatedOutlet from "./components/AnimatedOutlet.tsx";
import SideNav from "./components/SideNav/SideNav.tsx";
import styles from "./AppLayout.module.css";

function AppLayout(): JSX.Element {
  return (
    <div className={styles.appLayout}>
      <SideNav />
      <AnimatedOutlet />
    </div>
  );
}

export default AppLayout;

