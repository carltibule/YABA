import React from "react";
import { Routes, Route } from 'react-router-dom';
import { Footer, Header, ProtectedRoute } from './components';
import { BaseLayout, HomeView, RedirectView, BookmarksListView, BookmarkDetailView, TestView, NotFoundView, TagsView } from './views';
import { isDev } from "./utils";

function App() {
  return (
    <div className="App" style={{minHeight: '100vh', display: 'flex', flexDirection: 'column'}}>
      <Routes>
        <Route
          path="/"
          element={<BaseLayout header={ <Header />} content={ <HomeView /> } footer={ <Footer />}/>} 
        />

        <Route 
          path="/redirect"
          element={<BaseLayout header={ <Header />} content={ <RedirectView />} footer={ <Footer />}/> }
        />

        <Route
          path="/bookmarks"
          element={
            <ProtectedRoute 
              layout={BaseLayout}
              header={Header}
              footer={Footer}
              view={BookmarksListView}
              viewProps={{showHidden: false}}
            />
          }
        />

        <Route 
          path="/bookmarks/hidden"
          element={
            <ProtectedRoute 
              layout={BaseLayout}
              header={Header}
              footer={Footer}
              view={BookmarksListView}
              viewProps={{showHidden: true}}
            />
          }
        />

        <Route 
          path="/bookmarks/new"
          element={
            <ProtectedRoute 
              layout={BaseLayout}
              header={Header}
              footer={Footer}
              view={BookmarkDetailView}
            />
          }
        />

        <Route 
          path="/bookmarks/:id"
          element={
            <ProtectedRoute 
              layout={BaseLayout}
              header={Header}
              footer={Footer}
              view={BookmarkDetailView}
            />
          }
        />

        <Route
          path="/404"
          element={<BaseLayout content={<NotFoundView />} />}
        />

        { isDev() && (
          <Route
            path="/test"
            element={<BaseLayout header={ <Header />} content={ <TestView /> } footer={ <Footer />}/>} 
          />
        )}

        <Route
          path="/tags"
          element={
            <ProtectedRoute 
              layout={BaseLayout}
              header={Header}
              footer={Footer}
              view={TagsView}
            />
          } 
        />
      </Routes>
    </div>
  );
}

export default App;
