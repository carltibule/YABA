import React, { useState } from "react";
import { SplashScreen } from "../components";
import { Container, Col, Row } from "../components/external";
import 'bootstrap/dist/css/bootstrap.min.css';
import DateTimeHelper from "../utils/dateTimeHelper";
import { getTagGroups } from "../utils";

export function TestView() {
    const [showSplashScreen, setShowSplashScreen] = useState(false);
    const onCloseSplashScreenClick = () => setShowSplashScreen(false);
    const onShowSplashScreenClick = () => setShowSplashScreen(true);

    return(
        <React.Fragment>
            {showSplashScreen && (
                <SplashScreen 
                    onCloseSpashScreenClick={onCloseSplashScreenClick} 
                    showCloseSplashScreenButton={true}
                    message="Lorem ipsum dolor sit amet, consectetur adipiscing elit"
                />
            )}
            <div style={{flexGrow: 1}}>
                <Container>
                    {/* <Row>
                        <Col xs="3">
                            <Button variant="danger" onClick={onShowSplashScreenClick}>Show splash screen</Button>
                        </Col>
                    </Row> */}
                </Container>
            </div>
        </React.Fragment>
    );
}