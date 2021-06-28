package be.kdg.stemtest.model.entity
import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity
data class Statement(
   @PrimaryKey  val id: Int,
   val text:String,
   val explanation:String?
   )