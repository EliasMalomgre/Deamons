package be.kdg.stemtest.model.db.dao

import androidx.room.*
import be.kdg.stemtest.model.entity.CustomDebateGameResult
import io.reactivex.Maybe

@Dao
abstract class CustomDebateGameDao {

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    abstract fun insert(customDebateGameResult: CustomDebateGameResult)

    @Query("SELECT * FROM customdebategameresult WHERE id=:index")
    abstract fun getCustomDebateGameResult(index: Int) : Maybe<CustomDebateGameResult>


    @Transaction
   open  fun saveAllResults(results : List<CustomDebateGameResult>) {
        for (result in results) {
            insert(result)
        }
    }
}