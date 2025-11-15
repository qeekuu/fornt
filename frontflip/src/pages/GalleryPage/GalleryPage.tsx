import { useState } from "react";
import type { JSX } from "react";
import styles from "./GalleryPage.module.css";

type MockPhoto = {
  id: number;
  url: string | null;
};

function GalleryPage(): JSX.Element {
  const mockPhotos: MockPhoto[] = Array.from({ length: 12 }, (_, i) => ({
    id: i,
    url: null,
  }));

  const [previewPhoto, setPreviewPhoto] = useState<MockPhoto | null>(null);

  const openPreview = (photo: MockPhoto) => {
    setPreviewPhoto(photo);
  };

  const closePreview = () => {
    setPreviewPhoto(null);
  };

  return (
    <div className={styles.galleryPage}>
      <div className={styles.galleryInner}>

        <h1 className={styles.galleryTitle}>Galeria zdjęć</h1>

        <section className={styles.grid}>
          {mockPhotos.map((photo) => (
            <div key={photo.id} className={styles.photoCard}>
              <div className={styles.photoThumb}></div>

              <div className={styles.photoInfo}>
                <p className={styles.photoName}>Zdjęcie #{photo.id + 1}</p>

                <div className={styles.photoActions}>
                  <button
                    type="button"
                    className={styles.photoActionButton}
                    onClick={() => openPreview(photo)}
                  >
                    👁 Podgląd
                  </button>

                  <button type="button" className={styles.photoActionButton}>
                    🔗 Udostępnij
                  </button>

                  <button type="button" className={styles.photoActionButton}>
                    ⬇ Pobierz
                  </button>
                </div>
              </div>
            </div>
          ))}
        </section>
      </div>

      {previewPhoto && (
        <div className={styles.modalOverlay} onClick={closePreview}>
          <div
            className={styles.modalContent}
            onClick={(e) => e.stopPropagation()}
          >
            <button className={styles.modalClose} onClick={closePreview}>
              ✖
            </button>

            <div className={styles.modalImage}></div>
            {/* 
              <img src={previewPhoto.url} className={styles.modalImage} />
            */}
          </div>
        </div>
      )}
    </div>
  );
}

export default GalleryPage;

