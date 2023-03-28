import { withAuthenticationRequired } from "@auth0/auth0-react";
import React from "react";
import { SplashScreen } from "./shared/splashScreen";

export const ProtectedRoute = ({ layout, header, footer, view, viewProps }) => {
  const ProtectedView = withAuthenticationRequired(layout, {
    // onRedirecting: () => (
    //   <div className="page-layout">
    //     <SplashScreen message="You are being redirected to the authentication page" />
    //   </div>
    // ),
  });

  const Header = () => React.createElement(header);
  const Footer = () => React.createElement(footer);
  const View = () => React.createElement(view, viewProps);

  return <ProtectedView header={<Header/>} footer={<Footer />} content={<View />} />;
};