import { forwardRef } from "react";
import { Link } from "@tanstack/react-router";
import type { LinkProps } from "@tanstack/react-router";

// MUI needs a component that forwards refs
export const RouterLink = forwardRef<HTMLAnchorElement, LinkProps>((props, ref) => {
    return <Link {...props} ref={ref} />;
});
