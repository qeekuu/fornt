import {motion} from "framer-motion";
import type {JSX} from "react";

const variants = {
  initial: {opacity: 0, y: -20},
  animate: {opacity: 1, y: 0},
  exit: {opacity: 0, y: 20},
}

interface AnimatedPageProps {
  children: (JSX.Element | string | number)[] | JSX.Element | string | number,
  className?: string,
}

function AnimatedPage({children, className}: AnimatedPageProps): JSX.Element {
  return (
    <motion.main
      initial="initial"
      animate="animate"
      exit="exit"
      variants={variants}
      transition={{duration: 0.3}}
      className={className}
    >
      {children}
    </motion.main>
  );
}

export default AnimatedPage;
