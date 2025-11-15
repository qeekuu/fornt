import {cloneElement, type JSX} from "react";
import {useLocation, useOutlet} from "react-router-dom";
import {AnimatePresence} from "framer-motion";

function AnimatedOutlet(): JSX.Element {
  const location = useLocation();
  const outletElement = useOutlet();

  return <AnimatePresence mode={"wait"}>
    {outletElement && cloneElement(outletElement, {key: location.pathname})}
  </AnimatePresence>
}

export default AnimatedOutlet;
