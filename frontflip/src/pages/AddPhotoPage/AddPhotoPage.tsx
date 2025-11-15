import type { JSX } from "react";
import { useRef } from "react";
import styles from "./AddPhotoPage.module.css";

function AddPhotoPage(): JSX.Element {
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const handleFileSelect = () => {
    if (fileInputRef.current) {
      fileInputRef.current.click();
    }
  };

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    console.log("Wybrane zdjęcie:", file);
  };

  return (
    <div className={styles.addPage}>
      <div className={styles.addInner}>
        <h1 className={styles.addTitle}>Dodaj zdjęcie</h1>

        <div className={styles.uploadCard} onClick={handleFileSelect}>
          <div className={styles.uploadIcon}>+</div>
          <p>Kliknij, aby dodać zdjęcie</p>

          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            className={styles.fileInput}
            onChange={handleFileChange}
          />
        </div>
      </div>
    </div>
  );
}

export default AddPhotoPage;

